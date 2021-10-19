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

namespace OneClickCopy.OwnCopyLines
{
    public partial class OwnCopyLineListPanel : ItemsControl
    {
        private const int IsNotContainedItemView = -1;

        public static readonly DependencyProperty SetLatelyPointedCopyDataCommandProperty =
            DependencyProperty.Register("SetLatelyPointedCopyDataCommand", typeof(ICommand), typeof(OwnCopyLineListPanel), new PropertyMetadata(null));

        public ICommand SetLatelyPointedCopyDataCommand
        {
            get { return (ICommand)GetValue(SetLatelyPointedCopyDataCommandProperty); }
            set { SetValue(SetLatelyPointedCopyDataCommandProperty, value); }
        }

        public OwnCopyLineListPanel()
        {
            InitializeComponent();
        }

        private void OnNewCopyPointedFromClipboard(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is UIElement eventRaisedView) {
                int viewItemIndex = GetItemIndexOfTheView(eventRaisedView);

                if(viewItemIndex != IsNotContainedItemView)
                    SetLatelyPointedCopyDataCommand?.Execute(viewItemIndex);
            }
        }

        private int GetItemIndexOfTheView(UIElement view)
        {
            //This code is based on the scenario
            //that ItemCollection element is ViewModel and the itemview has the ViewModel in its DataContext.

            if (view is FrameworkElement fElement)
            {
                object dataContextOfTheView = fElement.DataContext;

                if (Items.Contains(dataContextOfTheView))
                    return Items.IndexOf(dataContextOfTheView);
            }

            return IsNotContainedItemView;
        }
    }
}
