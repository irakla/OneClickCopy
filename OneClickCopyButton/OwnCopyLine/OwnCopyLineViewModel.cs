using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Diagnostics;
using System.Resources;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace OneClickCopy.OwnCopyLine
{
    public class OwnCopyLineViewModel : DependencyObject
    {
        private OwnCopyData _ownCopyData = null;

        public static readonly DependencyProperty OwnCopyTitleProperty
            = DependencyProperty.Register("OwnCopyTitle", typeof(string), typeof(OwnCopyLinePanel));

        private bool isEdittingOwnCopyContent = false;
        private OwnCopyInfoPopup lastOwnCopyInfoPopup = null;

        private ToastNotifier _messageNotifier = null;
        private ResourceManager messageResourceManager = new ResourceManager(typeof(OneClickCopy.Properties.MessageResource));

        public event EventHandler PointedFromClipboard;

        public ICommand CopyToSystemClipboardCommand { get; }
        public ICommand OpenInfoPopupByCopyButtonCommand { get; }
        public ICommand OpenInfoPopupByEditButtonCommand { get; }

        public bool HasOwnCopyContent
        { get => (OwnCopyContent != null) && (OwnCopyContent.GetFormats().Length != 0); }

        public bool IsEditting
        {
            get => isEdittingOwnCopyContent;
            set
            {
                isEdittingOwnCopyContent = value;

                //move this part to XAML
                /*if (isEdittingOwnCopyContent)
                {
                    editButton.Style = (Style)Resources["EdittingLineButton"];
                }
                else
                {
                    editButton.Style = (Style)Resources["DefaultLineButton"];
                    if (lastOwnCopyInfoPopup != null)
                        lastOwnCopyInfoPopup.IsOpen = false;
                }*/
            }
        }

        public bool IsFixedTitle
        {
            get; set;
            /*get => titleFixingButton.IsChecked != null && (bool)titleFixingButton.IsChecked;
            set
            {
                titleFixingButton.IsChecked = value;

                if (value)
                    titleFixingPinEdge.Visibility = Visibility.Visible;
                else
                    titleFixingPinEdge.Visibility = Visibility.Hidden;
            }*/
        }

        public string OwnCopyTitle
        {
            get => (string)GetValue(OwnCopyTitleProperty);
            set => SetValue(OwnCopyTitleProperty, value);
        }

        public DataObject OwnCopyContent
        {
            get => _ownCopyData.Content;
            private set
            {
                _ownCopyData.Content = value;

                //move this part to XAML
                /*if (HasOwnCopyContent)
                    copyButton.Style = (Style)Resources["HasOwnCopyStyle"];
                else
                    copyButton.Style = (Style)Resources["DefaultLineButton"];*/
            }
        }

        public OwnCopyLineViewModel(object ownCopyData)
        {
            CopyToSystemClipboardCommand = new RelayCommand(CopyToSystemClipboard);
            OpenInfoPopupByCopyButtonCommand = new RelayCommand<Point>(OpenInfoPopupByCopyButton);
            OpenInfoPopupByEditButtonCommand = new RelayCommand<UIElement>(OpenInfoPopupByEditButton);

            GetMainWindowElements();
            InitializeOwnCopyData(ownCopyData);
        }

        private void GetMainWindowElements()
        {
            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;

            _messageNotifier = mainWindow?.messageNotifier;
        }

        private void InitializeOwnCopyData(object ownCopyData)
        {
            if (ownCopyData != null && ownCopyData is OwnCopyData)
                _ownCopyData = (OwnCopyData)ownCopyData;
            else
                _ownCopyData = new OwnCopyData();
        }

        public void ToggleTitleFixingButton(object sender, EventArgs e)
        {
            /*if (IsFixedTitle)
            {
                titleFixingPinEdge.Visibility = Visibility.Visible;

                string formattedFixedMessage = messageResourceManager.GetString("CopyButtonTitleFixed_Formatted");
                string nowTitle = ownCopyTitleText.Text;

                string titleIsFixedString = ownCopyTitleText.Text.Length <= ProperCharLengthForDisplaying ?
                    string.Format(formattedFixedMessage, nowTitle) :
                    string.Format(formattedFixedMessage, nowTitle.Substring(0, 10) + "..." + nowTitle.Substring(nowTitle.Length - 5));

                TryToLaunchThisMessage(titleIsFixedString);
            }
            else
            {
                titleFixingPinEdge.Visibility = Visibility.Hidden;

                string titleIsUnfixedString
                    = messageResourceManager.GetString("CopyButtonTitleUnfixed");

                TryToLaunchThisMessage(titleIsUnfixedString);
            }*/
        }

        public void OpenInfoPopupByCopyButton(Point cursorPosition)
        {
            IsEditting = false;

            Point nowCursorPosition = cursorPosition;

            CopyFromSystemClipboard();

            if (HasOwnCopyContent)
            {
                lastOwnCopyInfoPopup = new OwnCopyInfoPopup
                {
                    OwnCopyInfoPopupContent = OwnCopyContent
                };

                
                /*if (IsFixedTitle)
                    lastOwnCopyInfoPopup.titleTextBox.Text = ownCopyTitleText.Text;
                else
                    lastOwnCopyInfoPopup.SetTitleFromData();*/

                SetCopyInfoPopupCommon();
            }
        }

        private void CopyFromSystemClipboard()
        {
            bool BothCopiesAreEqual = HasOwnCopyContent && Clipboard.IsCurrent(OwnCopyContent);
            if (BothCopiesAreEqual)
            {
                TryToLaunchThisMessage(messageResourceManager.GetString("CopyButtonIsExistingData"));
                return;
            }

            IDataObject currentClipboardData = Clipboard.GetDataObject();

            if (currentClipboardData.GetFormats().Length == 0)
            {
                TryToLaunchThisMessage(messageResourceManager.GetString("CopyButtonClipboardIsEmpty"));
                return;
            }

            DataObject newCopy = new DataObject();

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
                        newCopy.SetData(nowFormat, nowCopyingData);
                }
                catch (System.Runtime.InteropServices.COMException comException)
                {
                    Debug.WriteLine("Skipped Data : " + nowFormat);
                    Debug.WriteLine("HResult : {0:X}, Message : {1}", comException.HResult, comException.Message);
                    skippedFormatCount++;
                }
            }

            if (newCopy.GetFormats().Length == skippedFormatCount)
                return;     //All format data is skipped so this copy doesn't contain anything.

            OwnCopyContent = newCopy;
            Clipboard.Clear();
            Clipboard.SetDataObject(OwnCopyContent);

            if (PointedFromClipboard != null)
                PointedFromClipboard(this, EventArgs.Empty);

            TryToLaunchThisMessage(messageResourceManager.GetString("CopyButtonSavedNewData"));
        }

        private void SetCopyInfoPopupCommon()
        {
            IsEditting = true;

            if (lastOwnCopyInfoPopup != null)
            {
                BindTitleTextControls();
                lastOwnCopyInfoPopup.Closed += ReportPopupClose;
            }
        }

        private void BindTitleTextControls()
        {
            var ownCopyInfoPopupTitleBinding = new Binding("Text");
            var bindingTitleTextBox = lastOwnCopyInfoPopup.titleTextBox;

            ownCopyInfoPopupTitleBinding.Source = bindingTitleTextBox;
            ownCopyInfoPopupTitleBinding.Mode = BindingMode.OneWay;
            ownCopyInfoPopupTitleBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;

            BindingOperations.SetBinding(this, OwnCopyTitleProperty, ownCopyInfoPopupTitleBinding);
        }

        private void ReportPopupClose(object reporter, EventArgs e)
        {
            if (reporter == lastOwnCopyInfoPopup)
                IsEditting = false;
        }

        public void CopyToSystemClipboard()
        {
            if (HasOwnCopyContent)
            {
                Clipboard.Clear();
                Clipboard.SetDataObject(OwnCopyContent);

                if (PointedFromClipboard != null)
                    PointedFromClipboard(this, EventArgs.Empty);

                TryToLaunchThisMessage(messageResourceManager.GetString("CopyButtonCopiedData"));
            }
            else
                TryToLaunchThisMessage(messageResourceManager.GetString("CopyButtonOwnCopyIsEmpty"));
        }

        public void OpenInfoPopupByEditButton(UIElement editButton)
        {
            if (!HasOwnCopyContent)
            {
                TryToLaunchThisMessage(messageResourceManager.GetString("EditButtonOwnCopyDoesntExist"));
                return;
            }

            IsEditting = false;
            lastOwnCopyInfoPopup = new OwnCopyInfoPopup(editButton);
            //lastOwnCopyInfoPopup.titleTextBox.Text = ownCopyTitleText.Text;
            SetCopyInfoPopupCommon();

            if (HasOwnCopyContent)
                lastOwnCopyInfoPopup.OwnCopyInfoPopupContent = OwnCopyContent;
        }

        private void TryToLaunchThisMessage(string message)
        {
            _messageNotifier?.LaunchTheMessage(message);
        }
    }
}
