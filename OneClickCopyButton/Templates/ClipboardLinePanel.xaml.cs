using OneClickCopy.Templates;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
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

        public void OnClipboardEditorByCopyButton(object sender, MouseEventArgs mouseEvent)
        {
            FlushLastClipboardEditor();

            Point nowCursorPosition = mouseEvent.GetPosition(Application.Current.MainWindow);

            lastClipboardEditor = new ClipboardEditor();
            IsEditting = true;

            if (lastClipboardEditor != null)
            {
                lastClipboardEditor.Closed += ReportSelfClose;
            }
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
                IsEditting = true;

                if (lastClipboardEditor != null)
                {
                    lastClipboardEditor.Closed += ReportSelfClose;
                }
            }
        }

        private void ReportSelfClose(object reporter, EventArgs e)
        {
            if (reporter == lastClipboardEditor)
                IsEditting = false;
        }
    }
}
