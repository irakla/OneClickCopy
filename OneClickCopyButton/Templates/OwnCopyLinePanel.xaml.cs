using OneClickCopy.Templates;
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

        public bool IsEditting
        {
            get => isEdittingOwnCopyContent;
            set
            {
                isEdittingOwnCopyContent = value;

                if (isEdittingOwnCopyContent)
                {
                    EditButton.Style = (Style)Resources["EdittingLineButton"];
                }
                else
                {
                    EditButton.Style = (Style)Resources["DefaultLineButton"];
                    if (lastOwnCopyInfoPopup != null)
                        lastOwnCopyInfoPopup.IsOpen = false;
                }
            }
        }

        private DataObject OwnCopy
        {
            get => currentOwnCopy;
            set
            {
                currentOwnCopy = value;

                if (HasOwnCopy)
                    CopyButton.Style = (Style)Resources["HasOwnCopyStyle"];
                else
                    CopyButton.Style = (Style)Resources["DefaultLineButton"];

                if (HasOwnCopy)
                    Debug.WriteLine("Style changed to HasOwnCopyStyle");
            }
        }

        public bool HasOwnCopy
        { get => (currentOwnCopy != null) 
                && (currentOwnCopy.GetFormats().Length != 0); }

        public bool IsFixedTitle
        {
            get => TitleFixingButton.IsChecked != null && (bool)TitleFixingButton.IsChecked;
            set 
            {
                TitleFixingButton.IsChecked = value;

                if (value)
                    TitleFixingPinEdge.Visibility = Visibility.Visible;
                else
                    TitleFixingPinEdge.Visibility = Visibility.Hidden;
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

            messageNotifier = mainWindow?.MessageNotifier;
        }

        public void ToggleTitleFixingButton(object sender, EventArgs e)
        {
            if(IsFixedTitle)
            {
                TitleFixingPinEdge.Visibility = Visibility.Visible;

                string formattedFixedMessage = messageResourceManager.GetString("CopyButtonTitleFixed_Formatted");
                string nowTitle = OwnCopyTitleText.Text;

                string titleIsFixedString = OwnCopyTitleText.Text.Length <= 15 ?
                    string.Format(formattedFixedMessage, nowTitle) :
                    string.Format(formattedFixedMessage, nowTitle.Substring(0, 10) + "..." + nowTitle.Substring(nowTitle.Length - 5));

                messageNotifier?.LaunchTheMessage(titleIsFixedString);
            }
            else
            {
                TitleFixingPinEdge.Visibility = Visibility.Hidden;

                string titleIsUnfixedString
                    = messageResourceManager.GetString("CopyButtonTitleUnfixed");

                messageNotifier?.LaunchTheMessage(titleIsUnfixedString);
            }
        }

        public void CopyToSystemClipboard(object sender, EventArgs e)
        {
            if (HasOwnCopy)
            {
                Clipboard.Clear();
                Clipboard.SetDataObject(OwnCopy);
            }
        }

        public void OpenInfoPopupByCopyButton(object sender, MouseEventArgs mouseEvent)
        {
            IsEditting = false;

            Point nowCursorPosition = mouseEvent.GetPosition(Application.Current.MainWindow);

            CopyFromSystemClipboard();

            if (HasOwnCopy)
            {
                lastOwnCopyInfoPopup = new OwnCopyInfoPopup();
                lastOwnCopyInfoPopup.OwnCopyInfoPopupContent = OwnCopy;

                if (IsFixedTitle)
                    lastOwnCopyInfoPopup.TitleTextBox.Text = OwnCopyTitleText.Text;
                else
                    lastOwnCopyInfoPopup.SetTitleFromData();

                SetCopyInfoPopupCommon();
            }
        }

        private void CopyFromSystemClipboard()
        {
            bool TheCopiesAreEqual = HasOwnCopy && Clipboard.IsCurrent(OwnCopy);
            if (TheCopiesAreEqual)
                return;

            IDataObject currentClipboardData = Clipboard.GetDataObject();

            if (currentClipboardData.GetFormats().Length == 0)
                return;

            DataObject newCopy = new DataObject();
            
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
                    Console.WriteLine("Skipped Data : " + nowFormat);
                    Console.WriteLine("HResult : {0:X}, Message : {1}", comException.HResult, comException.Message);
                }
            }

            OwnCopy = newCopy;
        }

        public void OpenInfoPopupByEditButton(object sender, EventArgs _)
        {
            IsEditting = false;
            lastOwnCopyInfoPopup = new OwnCopyInfoPopup(EditButton);
            lastOwnCopyInfoPopup.TitleTextBox.Text = OwnCopyTitleText.Text;
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
            var bindingTitleTextBox = lastOwnCopyInfoPopup.TitleTextBox;

            titleBinding.Source = bindingTitleTextBox;
            titleBinding.Mode = BindingMode.OneWay;
            titleBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;

            BindingOperations.SetBinding(OwnCopyTitleText, TextBlock.TextProperty, titleBinding);
        }

        private void ReportPopupClose(object reporter, EventArgs e)
        {
            if (reporter == lastOwnCopyInfoPopup)
                IsEditting = false;
        }

        /*public void SetOwnCopyPersisted(object sender, EventArgs e)
        {
            Debug.WriteLine("Unloaded! state : " + Clipboard.IsCurrent(OwnCopy));
            if (HasOwnCopy && Clipboard.IsCurrent(OwnCopy))
                Clipboard.SetDataObject(OwnCopy, true);
        }*/
    }
}
