using Microsoft.Toolkit.Mvvm.Input;
using OneClickCopy.CustomType;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Resources;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace OneClickCopy.OwnCopyLines
{
    public class OwnCopyLineViewModel : DependencyObject, INotifyPropertyChanged
    {
        private ResourceManager messageResourceManager =
            new ResourceManager(typeof(OneClickCopy.Properties.MessageResource));

        private OwnCopyData _ownCopyData = null;

        public static readonly DependencyProperty OwnCopyTitleProperty
            = DependencyProperty.Register(nameof(OwnCopyTitle), typeof(string), typeof(OwnCopyLineViewModel),
                new PropertyMetadata(string.Empty));

        private OwnCopyInfoPopup lastOwnCopyInfoPopup = null;

        public static readonly DependencyProperty IsInfoPopupOpenedProperty
            = DependencyProperty.Register(nameof(IsInfoPopupOpened), typeof(bool), typeof(OwnCopyLineViewModel),
                new PropertyMetadata(false));
        public static readonly DependencyProperty IsFixedTitleProperty
            = DependencyProperty.Register(nameof(IsFixedTitle), typeof(bool), typeof(OwnCopyLineViewModel),
                new PropertyMetadata(false, OnTitleFixingButtonToggled));

        public ICommand CopyOwnToSystemClipboardCommand { get; }
        public ICommand SaveCopyAndOpenInfoPopupCommand { get; }
        public ICommand OpenInfoPopupOnUIElementCommand { get; }
        public ICommand CloseInfoPopupCommand { get; }

        public MutableExecuteCommand NotifyPointedFromClipboardCommand { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        public bool HasOwnCopyContent
        { get => (OwnCopyContent != null) && (OwnCopyContent.GetFormats().Length != 0); }

        public bool HasTextData
        {
            get => HasOwnCopyContent ?
                    OwnCopyContent.GetFormats().Contains(DataFormats.Text) : false;
        }

        public bool IsInfoPopupOpened
        {
            get => (bool)GetValue(IsInfoPopupOpenedProperty);
            set => SetValue(IsInfoPopupOpenedProperty, value);
        }

        public bool IsFixedTitle
        {
            get => (bool)GetValue(IsFixedTitleProperty);
            set => SetValue(IsFixedTitleProperty, value);
        }

        public OwnCopyData OwnCopyData 
        {
            get => _ownCopyData;
            set
            {
                if (value != null)
                    _ownCopyData = value;
                else
                    _ownCopyData = new OwnCopyData();

                OwnCopyTitle = _ownCopyData.Title;
                OwnCopyContent = _ownCopyData.Content;
                OnPropertyChanged(nameof(HasOwnCopyContent));
            }
        }

        public string OwnCopyTitle
        {
            get => (string)GetValue(OwnCopyTitleProperty);
            set => SetValue(OwnCopyTitleProperty, value);
        }

        public DataObject OwnCopyContent
        {
            get => _ownCopyData.Content;
            private set => _ownCopyData.Content = value;
        }

        public OwnCopyLineViewModel() : this(null) { }

        public OwnCopyLineViewModel(OwnCopyData ownCopyData)
        {
            CopyOwnToSystemClipboardCommand = new RelayCommand(CopyOwnToSystemClipboard);
            SaveCopyAndOpenInfoPopupCommand = new RelayCommand(SaveCopyAndOpenInfoPopup);
            OpenInfoPopupOnUIElementCommand = new RelayCommand<UIElement>(OpenInfoPopupOnUIElement);
            CloseInfoPopupCommand = new RelayCommand<bool?>(CloseInfoPopup);

            NotifyPointedFromClipboardCommand = new MutableExecuteCommand();

            InitializeOwnCopyData(ownCopyData);
        }

        private void InitializeOwnCopyData(OwnCopyData ownCopyData)
        {
            if (ownCopyData != null)
                _ownCopyData = ownCopyData;
            else
                _ownCopyData = new OwnCopyData();

            OwnCopyTitle = _ownCopyData.Title;
        }

        public static void OnTitleFixingButtonToggled(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is OwnCopyLineViewModel viewModel)
                viewModel.NotifyTitleFixingButtonToggled((bool)e.NewValue);
        }

        private void NotifyTitleFixingButtonToggled(bool toggledResult)
        {
            if (toggledResult)
            {
                const int ProperTitleLengthForDisplaying = 15;

                string formattedFixedMessage = messageResourceManager.GetString("CopyButtonTitleFixed_Formatted");
                string nowTitle = OwnCopyTitle;

                string titleIsFixedString = nowTitle.Length <= ProperTitleLengthForDisplaying ?
                    string.Format(formattedFixedMessage, nowTitle) :
                    string.Format(formattedFixedMessage, nowTitle.Substring(0, 10) + "..." + nowTitle.Substring(nowTitle.Length - 5));

                ToastNotifier.TryToLaunchMessage(titleIsFixedString);
            }
            else
            {
                string titleIsUnfixedString
                    = messageResourceManager.GetString("CopyButtonTitleUnfixed");

                ToastNotifier.TryToLaunchMessage(titleIsUnfixedString);
            }
        }

        private void SaveCopyAndOpenInfoPopup()
        {
            IsInfoPopupOpened = false;
            SaveCopyFromSystemClipboard();

            if (HasOwnCopyContent)
            {
                lastOwnCopyInfoPopup = new OwnCopyInfoPopup(this);

                SetCopyInfoPopupCommon();
                OnPropertyChanged(nameof(HasOwnCopyContent));
            }
        }

        private void SaveCopyFromSystemClipboard()
        {
            bool isEqualToOwnCopy = HasOwnCopyContent && Clipboard.IsCurrent(OwnCopyContent);
            if (isEqualToOwnCopy)
            {
                ToastNotifier.TryToLaunchMessage(messageResourceManager.GetString("CopyButtonIsExistingData"));
                return;
            }

            IDataObject currentClipboardData = Clipboard.GetDataObject();

            if (currentClipboardData.GetFormats().Length == 0)
            {
                ToastNotifier.TryToLaunchMessage(messageResourceManager.GetString("CopyButtonClipboardIsEmpty"));
                return;
            }

            DataObject cloneOfClipboardData = new DataObject();

            int skippedFormatCount = 0;
            foreach (string nowFormat in currentClipboardData.GetFormats())
            {
                try
                {
#if DEBUG
                    Debug.WriteLine("Copying Format : " + nowFormat);
#endif
                    object nowCopyingData = currentClipboardData.GetData(nowFormat);
#if DEBUG
                    Debug.WriteLine("Contents : " + nowCopyingData);
#endif

                    if (nowCopyingData != null)
                        cloneOfClipboardData.SetData(nowFormat, nowCopyingData);
                }
                catch (System.Runtime.InteropServices.COMException comException)
                {
                    Debug.WriteLine("Skipped Data : " + nowFormat);
                    Debug.WriteLine("HResult : {0:X}, Message : {1}", comException.HResult, comException.Message);
                    skippedFormatCount++;
                }
            }

            if (cloneOfClipboardData.GetFormats().Length == skippedFormatCount)
                return;     //All format data is skipped so this copy doesn't contain anything.

            OwnCopyContent = cloneOfClipboardData;
            Clipboard.Clear();
            Clipboard.SetDataObject(OwnCopyContent);

            if (!IsFixedTitle)
                OwnCopyTitle = GetTitleFromOwnCopyContent();

            NotifyPointedFromClipboardCommand.Execute(null);

            ToastNotifier.TryToLaunchMessage(messageResourceManager.GetString("CopyButtonSavedNewData"));
        }

        private string GetTitleFromOwnCopyContent()
        {
            string newTitleText = String.Empty;

            if (HasTextData)
            {
                string dataRawText = (string)OwnCopyContent.GetData(DataFormats.Text);
                string[] splittedByNewLine = dataRawText.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);    

                for (int nowContentLineCount = 0; nowContentLineCount < splittedByNewLine.Length; nowContentLineCount++)
                {
                    string nowContentLineText = splittedByNewLine[nowContentLineCount].Trim('\n');

                    if (String.IsNullOrWhiteSpace(nowContentLineText))
                        continue;

                    newTitleText = nowContentLineText.Trim(new[] { ' ', '\t' });
                    break;
                }
            }

            return newTitleText;
        }

        private void CopyOwnToSystemClipboard()
        {
            if (HasOwnCopyContent)
            {
                Clipboard.Clear();
                Clipboard.SetDataObject(OwnCopyContent);

                NotifyPointedFromClipboardCommand.Execute(null);

                ToastNotifier.TryToLaunchMessage(messageResourceManager.GetString("CopyButtonCopiedData"));
            }
            else
                ToastNotifier.TryToLaunchMessage(messageResourceManager.GetString("CopyButtonOwnCopyIsEmpty"));
        }

        private void OpenInfoPopupOnUIElement(UIElement editButton)
        {
            if (!HasOwnCopyContent)
            {
                ToastNotifier.TryToLaunchMessage(messageResourceManager.GetString("EditButtonOwnCopyDoesntExist"));
                return;
            }

            IsInfoPopupOpened = false;

            if(editButton != null)
                lastOwnCopyInfoPopup = new OwnCopyInfoPopup(this, editButton);
            else
                lastOwnCopyInfoPopup = new OwnCopyInfoPopup(this);

            SetCopyInfoPopupCommon();
        }

        private void SetCopyInfoPopupCommon()
        {
            IsInfoPopupOpened = true;

            if (lastOwnCopyInfoPopup != null)
            {
                lastOwnCopyInfoPopup.Closed += ReportPopupClose;
            }
        }

        private void ReportPopupClose(object reporter, EventArgs e)
        {
            if (reporter == lastOwnCopyInfoPopup)
                IsInfoPopupOpened = false;
        }

        private void CloseInfoPopup(bool? considerMousePosition)
        {
            if (lastOwnCopyInfoPopup == null)
                return;

            bool hasNoProblemAboutMousePosition =
                considerMousePosition == null ?
                true :
                !(bool)considerMousePosition || !lastOwnCopyInfoPopup.IsMouseOver;

            if (hasNoProblemAboutMousePosition)
                lastOwnCopyInfoPopup.IsOpen = false;
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
