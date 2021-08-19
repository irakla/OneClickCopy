using OneClickCopy.Templates;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
        private IDataObject currentOwnCopy = null;

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
                }
            }
        }

        public bool HasOwnCopy { get => currentOwnCopy != null; }

        public ClipboardLinePanel()
        {
            InitializeComponent();
        }

        public void FlushLastClipboardEditor()
        {
            if (lastClipboardEditor != null)
                lastClipboardEditor.IsOpen = false;

            IsEditting = false;
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
                Clipboard.SetDataObject(currentOwnCopy, true);
            }
        }

        public void OnClipboardEditorByCopyButton(object sender, MouseEventArgs mouseEvent)
        {
            FlushLastClipboardEditor();

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

            currentOwnCopy = Clipboard.GetDataObject();
        }

        public void OnOffClipboardEditorByEditButton(object sender, EventArgs _)
        {
            if (IsEditting)
            {

            }
            else
            {
                FlushLastClipboardEditor();
                lastClipboardEditor = new ClipboardEditor(EditButton);
                SetClipboardEditorCommon();
            }
        }

        private void ReportSelfClose(object reporter, EventArgs e)
        {
            if (reporter == lastClipboardEditor)
                IsEditting = false;
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
    }
}
