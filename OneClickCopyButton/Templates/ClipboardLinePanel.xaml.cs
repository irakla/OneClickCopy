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
        public ClipboardLinePanel()
        {
            InitializeComponent();
        }

        public void OpenClipboardEditor(object sender, RoutedEventArgs e)
        {
            if(sender is Button)
            {
                Button buttonModifyContent = (Button)sender;
                ClipboardEditor ce = new ClipboardEditor(buttonModifyContent);
            }
        }
    }
}
