﻿using OneClickCopy.Templates;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Resources;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OneClickCopy
{
    public partial class OwnCopyLinePanel : Grid
    {
        private bool isEdittingOwnCopyContent = false;
        private OwnCopyInfoPopup lastOwnCopyInfoPopup= null;

        private DataObject currentOwnCopy = null;

        private ToastNotifier messageNotifier = null;                   //In your code using this, consider it can be null
        private ResourceManager messageResourceManager = new ResourceManager(typeof(OneClickCopy.Properties.MessageResource));

        public event EventHandler PointedFromClipboard;

        private const int ProperCharLengthForDisplaying = 15;

        public bool IsEditting
        {
            get => isEdittingOwnCopyContent;
            set
            {
                isEdittingOwnCopyContent = value;

                if (isEdittingOwnCopyContent)
                {
                    editButton.Style = (Style)Resources["EdittingLineButton"];
                }
                else
                {
                    editButton.Style = (Style)Resources["DefaultLineButton"];
                    if (lastOwnCopyInfoPopup != null)
                        lastOwnCopyInfoPopup.IsOpen = false;
                }
            }
        }

        public DataObject OwnCopy
        {
            get => currentOwnCopy;
            private set
            {
                currentOwnCopy = value;

                if (HasOwnCopy)
                    copyButton.Style = (Style)Resources["HasOwnCopyStyle"];
                else
                    copyButton.Style = (Style)Resources["DefaultLineButton"];
            }
        }

        public bool HasOwnCopy
        { get => (currentOwnCopy != null) && (currentOwnCopy.GetFormats().Length != 0); }

        public bool IsFixedTitle
        {
            get => titleFixingButton.IsChecked != null && (bool)titleFixingButton.IsChecked;
            set 
            {
                titleFixingButton.IsChecked = value;

                if (value)
                    titleFixingPinEdge.Visibility = Visibility.Visible;
                else
                    titleFixingPinEdge.Visibility = Visibility.Hidden;
            }
        }

        public OwnCopyLinePanel()
        {
            InitializeComponent();

            Loaded += GetMainWindowElements;
        }
        
        private void GetMainWindowElements(object sender, EventArgs e)
        {
            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;

            messageNotifier = mainWindow?.messageNotifier;
        }

        public void ToggleTitleFixingButton(object sender, EventArgs e)
        {
            if(IsFixedTitle)
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
            }
        }

        public void CopyToSystemClipboard(object sender, EventArgs e)
        {
            if (HasOwnCopy)
            {
                Clipboard.Clear();
                Clipboard.SetDataObject(OwnCopy);

                if (PointedFromClipboard != null)
                    PointedFromClipboard(this, EventArgs.Empty);

                TryToLaunchThisMessage(messageResourceManager.GetString("CopyButtonCopiedData"));
            }
            else
                TryToLaunchThisMessage(messageResourceManager.GetString("CopyButtonOwnCopyIsEmpty"));
        }

        public void OpenInfoPopupByCopyButton(object sender, MouseEventArgs mouseEvent)
        {
            IsEditting = false;

            Point nowCursorPosition = mouseEvent.GetPosition(Application.Current.MainWindow);

            CopyFromSystemClipboard();

            if (HasOwnCopy)
            {
                lastOwnCopyInfoPopup = new OwnCopyInfoPopup
                {
                    OwnCopyInfoPopupContent = OwnCopy
                };

                if (IsFixedTitle)
                    lastOwnCopyInfoPopup.titleTextBox.Text = ownCopyTitleText.Text;
                else
                    lastOwnCopyInfoPopup.SetTitleFromData();

                SetCopyInfoPopupCommon();
            }
        }

        private void CopyFromSystemClipboard()
        {
            bool BothCopiesAreEqual = HasOwnCopy && Clipboard.IsCurrent(OwnCopy);
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
            foreach(string nowFormat in currentClipboardData.GetFormats())
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
                catch (System.Runtime.InteropServices.COMException comException) {
                    Debug.WriteLine("Skipped Data : " + nowFormat);
                    Debug.WriteLine("HResult : {0:X}, Message : {1}", comException.HResult, comException.Message);
                    skippedFormatCount++;
                }
            }

            if (newCopy.GetFormats().Length == skippedFormatCount)
                return;     //All format data is skipped so this copy doesn't contain anything.

            OwnCopy = newCopy;
            Clipboard.Clear();
            Clipboard.SetDataObject(OwnCopy);

            if(PointedFromClipboard != null)
                PointedFromClipboard(this, EventArgs.Empty);

            TryToLaunchThisMessage(messageResourceManager.GetString("CopyButtonSavedNewData"));
        }

        public void OpenInfoPopupByEditButton(object sender, EventArgs e)
        {
            if (!HasOwnCopy)
            {
                TryToLaunchThisMessage(messageResourceManager.GetString("EditButtonOwnCopyDoesntExist"));
                return;
            }

            IsEditting = false;
            lastOwnCopyInfoPopup = new OwnCopyInfoPopup(editButton);
            lastOwnCopyInfoPopup.titleTextBox.Text = ownCopyTitleText.Text;
            SetCopyInfoPopupCommon();

            if(HasOwnCopy)
                lastOwnCopyInfoPopup.OwnCopyInfoPopupContent = OwnCopy;
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
            var titleBinding = new Binding("Text");
            var bindingTitleTextBox = lastOwnCopyInfoPopup.titleTextBox;

            titleBinding.Source = bindingTitleTextBox;
            titleBinding.Mode = BindingMode.OneWay;
            titleBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;

            BindingOperations.SetBinding(ownCopyTitleText, TextBlock.TextProperty, titleBinding);
        }

        private void TryToLaunchThisMessage(string message)
        {
            messageNotifier?.LaunchTheMessage(message);
        }

        private void ReportPopupClose(object reporter, EventArgs e)
        {
            if (reporter == lastOwnCopyInfoPopup)
                IsEditting = false;
        }
    }
}
