using System;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Data;

namespace OneClickCopy.OwnCopyLines
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
            => _viewModel.CopyOwnToSystemClipboardCommand.Execute(null);

        public void OpenInfoPopupByCopyButton(object sender, EventArgs e)
            => _viewModel.SaveCopyAndOpenInfoPopupCommand.Execute(null);

        public void OpenInfoPopupByEditButton(object sender, EventArgs e)
            => _viewModel.OpenInfoPopupOnUIElementCommand.Execute(editButton);
    }
}
