using OneClickCopy.CustomType;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace OneClickCopy.OwnCopyLines
{
    public partial class OwnCopyLinePanel : UserControl
    {
        public static readonly DependencyProperty CopyOwnToSystemClipboardCommandProperty =
            DependencyProperty.Register("CopyOwnToSystemClipboardCommand", typeof(ICommand), typeof(OwnCopyLinePanel), new PropertyMetadata(null));
        public static readonly DependencyProperty SaveCopyAndOpenInfoPopupCommandProperty =
            DependencyProperty.Register("SaveCopyAndOpenInfoPopupCommand", typeof(ICommand), typeof(OwnCopyLinePanel), new PropertyMetadata(null));
        public static readonly DependencyProperty OpenInfoPopupOnUIElementCommandProperty =
            DependencyProperty.Register("OpenInfoPopupOnUIElementCommand", typeof(ICommand), typeof(OwnCopyLinePanel), new PropertyMetadata(null));

        public static readonly DependencyProperty NotifyPointedFromClipboardCommandProperty =
            DependencyProperty.Register("NotifyPointedFromClipboardCommand", typeof(MutableExecuteCommand), typeof(OwnCopyLinePanel),
                new PropertyMetadata(null, OnNotifyPointedFromClipboardCommandPropertyChanged));

        public ICommand CopyOwnToSystemClipboardCommand
        {
            get { return (ICommand)GetValue(CopyOwnToSystemClipboardCommandProperty); }
            set { SetValue(CopyOwnToSystemClipboardCommandProperty, value); }
        }

        public ICommand SaveCopyAndOpenInfoPopupCommand
        {
            get { return (ICommand)GetValue(SaveCopyAndOpenInfoPopupCommandProperty); }
            set { SetValue(SaveCopyAndOpenInfoPopupCommandProperty, value); }
        }

        public ICommand OpenInfoPopupOnUIElementCommand
        {
            get { return (ICommand)GetValue(OpenInfoPopupOnUIElementCommandProperty); }
            set { SetValue(OpenInfoPopupOnUIElementCommandProperty, value); }
        }

        public static readonly RoutedEvent PointedFromClipboardEvent = EventManager.RegisterRoutedEvent(
            "PointedFromClipboard", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(OwnCopyLinePanel));

        public MutableExecuteCommand NotifyPointedFromClipboardCommand
        {
            get { return (MutableExecuteCommand)GetValue(NotifyPointedFromClipboardCommandProperty); }
            set { SetValue(NotifyPointedFromClipboardCommandProperty, value); }
        }

        public event RoutedEventHandler PointedFromClipboard
        {
            add { AddHandler(PointedFromClipboardEvent, value); }
            remove { RemoveHandler(PointedFromClipboardEvent, value); }
        }

        public OwnCopyLinePanel()
        {
            InitializeComponent();
        }

        public void CopyToSystemClipboard(object sender, EventArgs e)
            => CopyOwnToSystemClipboardCommand?.Execute(null);

        public void OpenInfoPopupByCopyButton(object sender, EventArgs e)
            => SaveCopyAndOpenInfoPopupCommand?.Execute(null);

        public void OpenInfoPopupByEditButton(object sender, EventArgs e)
            => OpenInfoPopupOnUIElementCommand?.Execute(editButton);

        public static void OnNotifyPointedFromClipboardCommandPropertyChanged(
            DependencyObject sender, 
            DependencyPropertyChangedEventArgs e
            )
        {
            if (sender is OwnCopyLinePanel ownCopyLinePanel)
            {
                ownCopyLinePanel.NotifyPointedFromClipboardCommand.MutableExecute =
                    () => ownCopyLinePanel.RaiseEvent(new RoutedEventArgs(PointedFromClipboardEvent));
            }
        }
    }
}
