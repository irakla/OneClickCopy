using OneClickCopy.OwnCopyLine;
using System;
using System.Diagnostics;
using System.Resources;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace OneClickCopy
{
    public partial class OwnCopyLinePanel : Grid
    {
        private bool isEdittingOwnCopyContent = false;
        private OwnCopyInfoPopup lastOwnCopyInfoPopup = null;

        private OwnCopyData _ownCopyData;

        private ToastNotifier _messageNotifier = null;
        private ResourceManager messageResourceManager = new ResourceManager(typeof(OneClickCopy.Properties.MessageResource));

        private OwnCopyLineViewModel _viewModel = null;

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

            Loaded += (o, e) =>
            {
                _viewModel = new OwnCopyLineViewModel(DataContext);
                DataContext = _viewModel;
            };
        }

        public void ToggleTitleFixingButton(object sender, EventArgs e)
        {
            /*if(IsFixedTitle)
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
            }*/
        }

        public void CopyToSystemClipboard(object sender, EventArgs e)
        {
            _viewModel.CopyToSystemClipboardCommand.Execute(null);
        }

        public void OpenInfoPopupByCopyButton(object sender, MouseEventArgs mouseEvent)
        {
            Point nowCursorPosition = mouseEvent.GetPosition(Application.Current.MainWindow);
            
            _viewModel.OpenInfoPopupByCopyButtonCommand.Execute(nowCursorPosition);
        }

        public void OpenInfoPopupByEditButton(object sender, EventArgs e)
        {
            _viewModel.OpenInfoPopupByEditButtonCommand.Execute(editButton);
        }
    }
}
