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

namespace OneClickCopy
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool isChangedLocationOnScreen = false;
        private bool isPinnedOnWindows = true;

        private SettingFileEntry settingFile;
        private WindowSettings nowWindowSettings;

        private const string SettingNameStartupLeftPosition = "StartupLeftPosition";
        private const string SettingNameStartupTopPosition = "StartupTopPosition";
        private const string SettingNameTopmostPinIsChecked = "TopmostPinIsChecked";
        private const string SettingNameTransparentlyIfCursorLeftIsChecked = "TransparentlyIfCursorLeftIsChecked";
        private const string SettingNameOpacityIfCursorLeft = "OpacityIfCursorLeft";

        public MainWindow()
        {
            InitializeComponent();
            nowWindowSettings = new WindowSettings(this);

            /*bool isSuccessfulReadingSettingFile = InitialSettingFileRead();

            if (isSuccessfulReadingSettingFile)
                ;//TODO 기존 설정파일 읽기 성공(SettingFileEntry가 읽어낸 세팅 덧씌움)
            else
            {
                Properties.Settings.Default.LeftOnScreen.ToString();
                Properties.Settings.Default.
                ;//TODO 기존 설정파일이 없음(초기설정으로 세팅)
            }*/

            bool isExistPrevSetting = Properties.Settings.Default.IsExistPrevSetting;
            if (isExistPrevSetting){
                Left = Properties.Settings.Default.LeftOnScreen;
                Top = Properties.Settings.Default.TopOnScreen;
            }

            Properties.Settings.Default.IsExistPrevSetting = true;
            Properties.Settings.Default.Save();
        }

        private bool InitialSettingFileRead()
        {
            SetNowWindowPin();

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
            catch (IOException e)
            {
                //TODO : 기존 환경설정 파일 읽기 실패시 예외처리
                return false;
            }

            return true;
        }

        private void IsClickedPin(object sender, RoutedEventArgs e)
        {
            isPinnedOnWindows = !isPinnedOnWindows;
            SetNowWindowPin();
        }

        private void IsClickedForDrag(object sender, MouseEventArgs mouseEventArgs)
        {
            if (mouseEventArgs.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
                ApplyLocationToProperty();
            }
        }

        private void IsMouseLeave(object sender, RoutedEventArgs e)
        {
            if (checkboxOpacity.IsChecked.GetValueOrDefault())
                Opacity = sliderOpacity.Value;
        }

        private void IsMouseEnter(object sender, RoutedEventArgs e)
        {
            Opacity = 1;
        }

        private void IsMouseTest(object sender, MouseEventArgs e)
        {
            Console.WriteLine(e.GetPosition(this));
        }

        private void SetNowWindowPin()
        {
            Topmost = isPinnedOnWindows;

            if (isPinnedOnWindows)
                pinEdge.Visibility = Visibility.Visible;
            else
                pinEdge.Visibility = Visibility.Hidden;
        }


        private void NotifyIsChangedLocationOnScreen(object sender, EventArgs changedEvent) => isChangedLocationOnScreen = true;

        private void ApplyLocationToProperty()
        {
            if (isChangedLocationOnScreen)
            {
                Properties.Settings.Default.LeftOnScreen = Left;
                Properties.Settings.Default.TopOnScreen = Top;

                Console.WriteLine(Properties.Settings.Default.LeftOnScreen + ", " + Properties.Settings.Default.TopOnScreen);
                Properties.Settings.Default.Save();

                isChangedLocationOnScreen = false;
            }
        }

        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = true;

        private void CommandBinding_CloseWindow(object sender, ExecutedRoutedEventArgs e) => SystemCommands.CloseWindow(this);

        private void CommandBinding_MinimizeWindow(object sender, ExecutedRoutedEventArgs e) => SystemCommands.MinimizeWindow(this);
    }
}
