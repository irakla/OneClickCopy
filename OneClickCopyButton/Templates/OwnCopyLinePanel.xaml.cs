using OneClickCopy.Templates;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
            }
        }

        public bool HasOwnCopy
        { get => (currentOwnCopy != null) && (currentOwnCopy.GetFormats().Length != 0); }

        public OwnCopyLinePanel()
        {
            InitializeComponent();
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
                SetCopyInfoPopupCommon();
                lastOwnCopyInfoPopup.OwnCopyInfoPopupContent = OwnCopy;
            }
        }

        private void CopyFromSystemClipboard()
        {
            bool TheCopiesAreEqual = HasOwnCopy && Clipboard.IsCurrent(OwnCopy);
            if (TheCopiesAreEqual)
                return;

            IDataObject currentClipboardData = Clipboard.GetDataObject();
            OwnCopy = new DataObject();
            
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
                        OwnCopy.SetData(nowFormat, nowCopyingData);
                }
                catch (System.Runtime.InteropServices.COMException comException) {
                    Console.WriteLine("Skipped Data : " + nowFormat);
                    Console.WriteLine("HResult : {0:X}, Message : {1}", comException.HResult, comException.Message);
                }
            }
        }

        public void OnOwnCopyInfoPopupByEditButton(object sender, EventArgs _)
        {
            IsEditting = false;
            lastOwnCopyInfoPopup = new OwnCopyInfoPopup(EditButton);
            SetCopyInfoPopupCommon();

            if(HasOwnCopy)
                lastOwnCopyInfoPopup.OwnCopyInfoPopupContent = OwnCopy;
        }

        private void SetCopyInfoPopupCommon()
        {
            IsEditting = true;

            BindTitleTextControls();

            if (lastOwnCopyInfoPopup != null)
            {
                lastOwnCopyInfoPopup.Closed += ReportSelfClose;
            }
        }

        private void BindTitleTextControls()
        {
            var titleBinding = new Binding("Text");
            var bindingTitleTextBox = lastOwnCopyInfoPopup.TitleTextBox;
            bindingTitleTextBox.Text = OwnCopyTitleText.Text;

            titleBinding.Source = bindingTitleTextBox;
            titleBinding.Mode = BindingMode.OneWay;
            titleBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;

            BindingOperations.SetBinding(OwnCopyTitleText, TextBlock.TextProperty, titleBinding);
        }

        private void ReportSelfClose(object reporter, EventArgs e)
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
