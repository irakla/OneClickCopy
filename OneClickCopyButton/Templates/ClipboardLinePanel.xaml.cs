using OneClickCopy.Templates;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
                /*Thread thread = new Thread(() =>
                {
                    Clipboard.SetDataObject(currentOwnCopy, true);
                });
                thread.SetApartmentState(ApartmentState.STA);

                thread.Start();*/
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

                foreach (string nowFormat in currentOwnCopy.GetFormats(false))
                {
                    Debug.WriteLine("Can be : " + nowFormat);
                }   //test
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
                Debug.WriteLine("Can be : " + nowFormat);
                object nowCopyingData = currentClipboardData.GetData(nowFormat);
                Debug.WriteLine("Contents : " + nowCopyingData);
                if (nowCopyingData != null)
                    currentOwnCopy.SetData(nowFormat, nowCopyingData);
            }

            byte[] bytes = null;

            var binarySerializer = new BinaryFormatter();
            var serializedOwnCopy = binarySerializer.Serialize((new SerializableDataObject(currentOwnCopy)));
        }

        public void OnOffClipboardEditorByEditButton(object sender, EventArgs _)
        {
            if (IsEditting)
            {

            }
            else
            {
                IsEditting = false;
                lastClipboardEditor = new ClipboardEditor(EditButton);
                SetClipboardEditorCommon();

                if(HasOwnCopy)
                    lastClipboardEditor.ClipboardEditorContent = currentOwnCopy;
            }
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

        public void SetOwnCopyPersisted(object sender, EventArgs e)
        {
            Debug.WriteLine("Unloaded! state : " + Clipboard.IsCurrent(currentOwnCopy));
            if (HasOwnCopy && Clipboard.IsCurrent(currentOwnCopy))
                Clipboard.SetDataObject(currentOwnCopy, true);
        }
    }
}
