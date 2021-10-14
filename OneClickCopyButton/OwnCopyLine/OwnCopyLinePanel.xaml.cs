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
        private OwnCopyLineViewModel _viewModel = null;

        public OwnCopyLinePanel()
        {
            InitializeComponent();

            Loaded += SwitchDataContextToViewModel;
        }

        private void SwitchDataContextToViewModel(object sender, EventArgs e)
        {
            _viewModel = new OwnCopyLineViewModel(DataContext);
            DataContext = _viewModel;
        }

        public void CopyToSystemClipboard(object sender, EventArgs e)
            => _viewModel.CopyToSystemClipboardCommand.Execute(null);

        public void OpenInfoPopupByCopyButton(object sender, MouseEventArgs mouseEvent)
        {
            Point nowCursorPosition = mouseEvent.GetPosition(Application.Current.MainWindow);
            
            _viewModel.OpenInfoPopupByCopyButtonCommand.Execute(nowCursorPosition);
        }

        public void OpenInfoPopupByEditButton(object sender, EventArgs e)
            => _viewModel.OpenInfoPopupByEditButtonCommand.Execute(editButton);
    }
}
