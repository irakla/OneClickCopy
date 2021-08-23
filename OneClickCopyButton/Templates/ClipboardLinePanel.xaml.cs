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
    /// <summary>
    /// ClipboardLinePanel.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ClipboardLinePanel : Grid
    {
        private bool isEdittingClipboardContent = false;
        private ClipboardEditor lastClipboardEditor = null;
        private DataObject currentOwnCopy = null;

        public bool IsEditting
        {
            get => isEdittingClipboardContent;
            set
            {
                isEdittingClipboardContent = value;

                if (isEdittingClipboardContent)
                {
                    EditButton.Style = (Style)Resources["EdittingLineButton"];
                }
                else
                {
                    EditButton.Style = (Style)Resources["DefaultLineButton"];
                    if (lastClipboardEditor != null)
                        lastClipboardEditor.IsOpen = false;
                }
            }
        }

        public bool HasOwnCopy { get => currentOwnCopy != null; }

        public ClipboardLinePanel()
        {
            InitializeComponent();
        }

        public void CopyToSystemClipboard(object sender, EventArgs e)
        {
            if (HasOwnCopy)
            {
                Clipboard.Clear();
                Clipboard.SetDataObject(currentOwnCopy);
            }
        }

        public void OnClipboardEditorByCopyButton(object sender, MouseEventArgs mouseEvent)
        {
            IsEditting = false;

            Point nowCursorPosition = mouseEvent.GetPosition(Application.Current.MainWindow);

            CopyFromSystemClipboard();

            if (HasOwnCopy)
            {
                lastClipboardEditor = new ClipboardEditor();
                SetClipboardEditorCommon();
                lastClipboardEditor.ClipboardEditorContent = currentOwnCopy;
            }
        }

        private void CopyFromSystemClipboard()
        {
            bool TheCopiesAreEqual = HasOwnCopy && Clipboard.IsCurrent(currentOwnCopy);
            if (TheCopiesAreEqual)
                return;

            IDataObject currentClipboardData = Clipboard.GetDataObject();
            currentOwnCopy = new DataObject();
            
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
                        currentOwnCopy.SetData(nowFormat, nowCopyingData);
                }
                catch (System.Runtime.InteropServices.COMException comException) {
                    Debug.WriteLine("Skipped Data : " + nowFormat);
                    Debug.WriteLine("HResult : {0:X}, Message : {1}", comException.HResult, comException.Message);
                }
            }
        }

        public void OnClipboardEditorByEditButton(object sender, EventArgs _)
        {
            IsEditting = false;
            lastClipboardEditor = new ClipboardEditor(EditButton);
            SetClipboardEditorCommon();

            if(HasOwnCopy)
                lastClipboardEditor.ClipboardEditorContent = currentOwnCopy;
        }

        private void SetClipboardEditorCommon()
        {
            IsEditting = true;

            BindTitleTextControls();

            if (lastClipboardEditor != null)
            {
                lastClipboardEditor.Closed += ReportSelfClose;
            }
        }

        private void BindTitleTextControls()
        {
            var titleBinding = new Binding("Text");
            var bindingTitleTextBox = lastClipboardEditor.TitleTextBox;
            bindingTitleTextBox.Text = ClipboardTitleText.Text;

            titleBinding.Source = bindingTitleTextBox;
            titleBinding.Mode = BindingMode.OneWay;
            titleBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;

            BindingOperations.SetBinding(ClipboardTitleText, TextBlock.TextProperty, titleBinding);
        }

        private void ReportSelfClose(object reporter, EventArgs e)
        {
            if (reporter == lastClipboardEditor)
                IsEditting = false;
        }

        /*public void SetOwnCopyPersisted(object sender, EventArgs e)
        {
            Debug.WriteLine("Unloaded! state : " + Clipboard.IsCurrent(currentOwnCopy));
            if (HasOwnCopy && Clipboard.IsCurrent(currentOwnCopy))
                Clipboard.SetDataObject(currentOwnCopy, true);
        }*/
    }
}
