using System;
using System.Collections.Generic;
using System.IO;
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

namespace OneClickCopyButton
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool isPinnedOnWindows = true;

        private SettingFileEntry settingFile;

        public MainWindow()
        {
            InitializeComponent();

            bool isSuccessfulReadingSettingFile = InitialSettingFileRead();

            //if (!isSuccessfulReadingSettingFile)
                
        }

        private bool InitialSettingFileRead()
        {
            setNowWindowPin();

            string nowPath = System.Environment.CurrentDirectory;

            if (string.IsNullOrEmpty(nowPath))
            {
                MessageBox.Show("환경설정 파일 경로를 찾지 못했습니다.(현재 경로 식별 실패)");
                return false;
            }

            try
            {
                settingFile = new SettingFileEntry(this, nowPath);
            }
            catch(IOException e)
            {
                return false;
            }

            return true;
        }

        private void isClickedPin(object sender, RoutedEventArgs e)
        {
            isPinnedOnWindows = !isPinnedOnWindows;
            setNowWindowPin();
        }

        private void isClickedForDrag(object sender, MouseEventArgs mouseEventArgs)
        {
            if(mouseEventArgs.LeftButton == MouseButtonState.Pressed)
                this.DragMove();
        }

        private void isMouseLeave(object sender, RoutedEventArgs e)
        {
            Opacity = 0.25;
        }

        private void isMouseEnter(object sender, RoutedEventArgs e)
        {
            Opacity = 1;
        }

        private void isMouseTest(object sender, MouseEventArgs e)
        {
            Console.WriteLine(e.GetPosition(this));
        }

        private void setNowWindowPin()
        {
            Topmost = isPinnedOnWindows;

            if (isPinnedOnWindows)
                pinEdge.Visibility = Visibility.Visible;
            else
                pinEdge.Visibility = Visibility.Hidden;
        }

        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CommandBinding_CloseWindow(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }

        private void CommandBinding_MinimizeWindow(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MinimizeWindow(this);
        }
    }
}
