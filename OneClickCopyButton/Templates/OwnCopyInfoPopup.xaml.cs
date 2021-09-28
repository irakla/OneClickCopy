using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OneClickCopy.Templates
{
    public partial class OwnCopyInfoPopup : Popup
    {
        private IDataObject nowOwnCopyData = null;

        private Window currentMainWindow = Application.Current.MainWindow;

        public bool HasTextData
        {
            get
            {
                return nowOwnCopyData != null ?
                    nowOwnCopyData.GetFormats().Contains(DataFormats.Text) : false;
            }
        }

        public IDataObject OwnCopyInfoPopupContent
        {
            get => nowOwnCopyData;
            set
            {
                nowOwnCopyData = value;

                if (HasTextData)
                {
                    if (OwnCopyContentLabel.Content is TextBox)
                    {
                        TextBox nowTextBox = (TextBox)OwnCopyContentLabel.Content;
                        nowTextBox.Text += (string)value.GetData(DataFormats.Text);
                    }
                }
            }
        }

        public OwnCopyInfoPopup()
        {
            InitializeComponent();

            this.Placement = PlacementMode.MousePoint;

            OpenInfoPopup();
        }

        public OwnCopyInfoPopup(UIElement placementTarget)
        {
            InitializeComponent();

            this.PlacementTarget = placementTarget;

            OpenInfoPopup();
        }

        public void SetTitleFromData()
        {
            if (HasTextData)
            {
                string dataRawText = (string)OwnCopyInfoPopupContent.GetData(DataFormats.Text);
                string[] splittedByNewLine = dataRawText.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

                string newTitleText = String.Empty; 
                
                for(int nowContentLineCount = 0; nowContentLineCount < splittedByNewLine.Length; nowContentLineCount++)
                {
                    string nowContentLineText = splittedByNewLine[nowContentLineCount].Trim('\n');

                    if (String.IsNullOrWhiteSpace(nowContentLineText))
                        continue;

                    newTitleText = nowContentLineText.Trim(new[] { ' ', '\t' });
                    break;
                }

                TitleTextBox.Text = newTitleText;
                TitleTextBox.SelectAll();
            }
        }

        private void OpenInfoPopup()
        {
            IsOpen = true;
            StaysOpen = true;

            SubscribeEventsClosingInfoPopup();
            
            ShowTextContent();

            TitleTextBox.Focus();
            TitleTextBox.SelectAll();
        }

        private void ShowTextContent()
        {
            TextBox txtboxCopyingContent = new TextBox();
            txtboxCopyingContent.HorizontalAlignment = HorizontalAlignment.Stretch;
            txtboxCopyingContent.MinLines = 3;
            txtboxCopyingContent.IsReadOnly = true;
            txtboxCopyingContent.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            txtboxCopyingContent.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            OwnCopyContentLabel.Content = txtboxCopyingContent;
        }

        private void CloseInfoPopup(object sender, EventArgs e)
        {
            if (IsMouseOver)
                return;

            EnterProcedurePopupClosing();
        }

        private void EnterProcedurePopupClosing()
        {
            BindingOperations.ClearBinding(TitleTextBox, TextBlock.TextProperty);
            IsOpen = false;

            UnsubscribeEventsClosingInfoPopup();
        }

        private void SubscribeEventsClosingInfoPopup()
        {
            LostFocus += CloseInfoPopup;
            currentMainWindow.Deactivated += CloseInfoPopup;
            currentMainWindow.PreviewMouseDown += CloseInfoPopup;
            currentMainWindow.PreviewMouseRightButtonDown += CloseInfoPopup;
        }

        private void UnsubscribeEventsClosingInfoPopup()
        {
            LostFocus -= CloseInfoPopup;
            currentMainWindow.Deactivated -= CloseInfoPopup;
            currentMainWindow.PreviewMouseDown -= CloseInfoPopup;
            currentMainWindow.PreviewMouseRightButtonDown -= CloseInfoPopup;
        }

        private void KeyInteraction(object sender, KeyEventArgs keyEvent)
        {
            if (keyEvent.Key == Key.Enter)
                EnterProcedurePopupClosing();
        }

        private void MouseTest(object sender, MouseEventArgs e)
        {
            Debug.WriteLine(e.GetPosition(this));
        }

        private void ResponseTest(object sender, EventArgs e)
        {
            Debug.WriteLine("I think This Window Lost focus");
        }

        ~OwnCopyInfoPopup()
        {
            Debug.WriteLine("It's Dead");
        }
    }
}
