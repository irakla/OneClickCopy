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
    /// <summary>
    /// ClipboardEditor.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ClipboardEditor : Popup
    {
        private bool isBinaryData = false;

        private Window currentMainWindow =Application.Current.MainWindow;

        public ClipboardEditor()
        {
            InitializeComponent();

            this.Placement = PlacementMode.MousePoint;

            OpenEditor();
        }

        public ClipboardEditor(UIElement placementTarget)
        {
            InitializeComponent();

            this.PlacementTarget = placementTarget;

            OpenEditor();
        }

        private void OpenEditor()
        {
            IsOpen = true;
            StaysOpen = true;

            SubscribeEventsClosingEditor();

            if (isBinaryData)
                ;//TODO : Draw the binary data in label
            else
            {
                TextBox txtboxCopyingContent = new TextBox();
                txtboxCopyingContent.HorizontalAlignment = HorizontalAlignment.Stretch;
                txtboxCopyingContent.MinLines = 3;
                EditorGrid.Children.Add(txtboxCopyingContent);
                Grid.SetRow(txtboxCopyingContent, 3);
                txtboxCopyingContent.Focus();
            }
        }

        private void CloseEditor(object sender, EventArgs e)
        {
            if (IsMouseOver)
                return;

            IsOpen = false;

            if (e is RoutedEventArgs)
                Debug.WriteLine("LostFocus!");
            if (e is MouseButtonEventArgs)
                Debug.WriteLine("MouseDown!");
                
            UnsubscribeEventsClosingEditor();
        }

        private void SubscribeEventsClosingEditor()
        {
            LostFocus += CloseEditor;
            currentMainWindow.Deactivated += CloseEditor;
            currentMainWindow.PreviewMouseDown += CloseEditor;
            currentMainWindow.PreviewMouseRightButtonDown += CloseEditor;
        }

        private void UnsubscribeEventsClosingEditor()
        {
            LostFocus -= CloseEditor;
            currentMainWindow.Deactivated -= CloseEditor;
            currentMainWindow.PreviewMouseDown -= CloseEditor;
            currentMainWindow.PreviewMouseRightButtonDown -= CloseEditor;
        }

        private void MouseTest(object sender, MouseEventArgs e)
        {
            Debug.WriteLine(e.GetPosition(this));
        }

        private void ResponseTest(object sender, EventArgs e)
        {
            Debug.WriteLine("I think This Window Lost focus");
        }

        ~ClipboardEditor()
        {
            Debug.WriteLine("It's Dead");
        }
    }
}
