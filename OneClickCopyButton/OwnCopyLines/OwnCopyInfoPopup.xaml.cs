using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;

namespace OneClickCopy.OwnCopyLines
{
    public partial class OwnCopyInfoPopup : Popup
    {
        private Window currentMainWindow = Application.Current.MainWindow;

        public OwnCopyLineViewModel ViewModel { get => DataContext as OwnCopyLineViewModel; }

        public DataObject ShowingCopyDataContent { get => ViewModel?.OwnCopyContent; }

        public OwnCopyInfoPopup(object viewModel, UIElement placementTarget = null)
        {
            //This viewModel is reused instance for existing ViewModel.
            DataContext = viewModel;

            InitializeComponent();

            if (placementTarget == null)
                this.Placement = PlacementMode.MousePoint;
            else
                this.PlacementTarget = placementTarget;

            OpenInfoPopup();
        }

        private void OpenInfoPopup()
        {
            IsOpen = true;
            StaysOpen = true;

            SubscribeEventsClosingInfoPopup();
            Closed += UnsubscribeEventsClosingInfoPopup;

            if(ViewModel != null && ViewModel.HasTextData)
                ShowTextContent();

            titleTextBox.Focus();
            titleTextBox.SelectAll();
        }

        private void ShowTextContent()
        {
            var textContentPresenter = Resources["TextContentPresenter"] as TextBox;
            
            if (textContentPresenter != null)
            {
                ownCopyContentLabel.Content = textContentPresenter;

                if(ShowingCopyDataContent != null)
                    textContentPresenter.Text += ShowingCopyDataContent.GetData(DataFormats.Text);
            }
        }

        private void CloseInfoPopup(object sender, EventArgs e)
            => ViewModel?.CloseInfoPopupCommand.Execute(false);

        private void CloseInfoPopupWithConsideringMousePosition(object sender, EventArgs e)
            => ViewModel?.CloseInfoPopupCommand.Execute(true);

        private void SubscribeEventsClosingInfoPopup()
        {
            LostFocus += CloseInfoPopupWithConsideringMousePosition;
            currentMainWindow.Deactivated += CloseInfoPopup;
            currentMainWindow.PreviewMouseDown += CloseInfoPopup;
            currentMainWindow.PreviewMouseRightButtonDown += CloseInfoPopup;
        }

        private void UnsubscribeEventsClosingInfoPopup(object sender, EventArgs e)
        {
            LostFocus -= CloseInfoPopupWithConsideringMousePosition;
            currentMainWindow.Deactivated -= CloseInfoPopup;
            currentMainWindow.PreviewMouseDown -= CloseInfoPopup;
            currentMainWindow.PreviewMouseRightButtonDown -= CloseInfoPopup;
        }
    }
}
