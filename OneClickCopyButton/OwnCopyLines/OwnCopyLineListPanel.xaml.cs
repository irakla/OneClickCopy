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

        public static readonly DependencyProperty AddNewLineAtTheIndexCommandProperty =
            DependencyProperty.Register("AddNewLineAtTheIndexCommand", typeof(ICommand), typeof(OwnCopyLineListPanel), new PropertyMetadata(null));
        public static readonly DependencyProperty RemoveLineAtTheIndexCommandProperty =
            DependencyProperty.Register("RemoveLineAtTheIndexCommand", typeof(ICommand), typeof(OwnCopyLineListPanel), new PropertyMetadata(null));
        public static readonly DependencyProperty SetLatelyPointedCopyDataCommandProperty =
            DependencyProperty.Register("SetLatelyPointedCopyDataCommand", typeof(ICommand), typeof(OwnCopyLineListPanel), new PropertyMetadata(null));


        public ICommand AddNewLineAtTheIndexCommand
        {
            get { return (ICommand)GetValue(AddNewLineAtTheIndexCommandProperty); }
            set { SetValue(AddNewLineAtTheIndexCommandProperty, value); }
        }

        public ICommand RemoveLineAtTheIndexCommand
        {
            get { return (ICommand)GetValue(RemoveLineAtTheIndexCommandProperty); }
            set { SetValue(RemoveLineAtTheIndexCommandProperty, value); }
        }

        public ICommand SetLatelyPointedCopyDataCommand
        {
            get { return (ICommand)GetValue(SetLatelyPointedCopyDataCommandProperty); }
            set { SetValue(SetLatelyPointedCopyDataCommandProperty, value); }
        }

        public OwnCopyLineListPanel()
        {
            InitializeComponent();
        }

        private void OnPanelInteractionButtonClicked(object sender, RoutedEventArgs e)
        {
            if(e.OriginalSource is FrameworkElement clickedButton)
            {
                int viewLineIndex = GetLineIndexOfTheView(clickedButton);

                if (viewLineIndex == IsNotContainedItemView)
                    return;

                switch (clickedButton.Name)
                {
                    case "addNewLineButton":
                        AddNewLineAtTheIndexCommand?.Execute(viewLineIndex);
                        break;
                    case "removeThisLineButton":
                        RemoveLineAtTheIndexCommand?.Execute(viewLineIndex);
                        break;
#if DEBUG
                    default:
                        Debug.WriteLine("This is not Panel Interaction Button Name : " + clickedButton.Name);
                        break;
#endif
                }
            }
        }

        private void OnNewCopyPointedFromClipboard(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is UIElement eventRaisedView)
            {
                int viewLineIndex = GetLineIndexOfTheView(eventRaisedView);

                if (viewLineIndex != IsNotContainedItemView)
                    SetLatelyPointedCopyDataCommand?.Execute(viewLineIndex);
            }

            e.Handled = true;
        }

        private int GetLineIndexOfTheView(UIElement view)
        {
            //This code is based on the scenario
            //that ItemCollection element is ViewModel and the itemview has the ViewModel in its DataContext.

            if (view is FrameworkElement fElement)
            {
                object dataContextOfTheView = fElement.DataContext;

                if (Items.Contains(dataContextOfTheView))
                    return Items.IndexOf(dataContextOfTheView);

                else
                {
                    //Trace parent hierarchy and find a view that has LineViewModel in its DataContext.
                    DependencyObject nowElement = fElement.Parent;

                    while(!(nowElement is Window))
                    {
                        if (nowElement is FrameworkElement nowFElement &&
                            Items.Contains(nowFElement.DataContext))
                        {   //Line Index is found.
                            return Items.IndexOf(nowFElement.DataContext);
                        }
                        else if (nowElement is FrameworkElement)
                        {   //Line Index is not found. Trace parent.
                            nowElement = ((FrameworkElement)nowElement).Parent;
                            continue;
                        }
                        else
                            //Can't trace parent hierarchy no more.
                            return IsNotContainedItemView;
                    }
                }
            }

            return IsNotContainedItemView;
        }
    }
}
