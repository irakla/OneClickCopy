using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
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
        private bool isPinnedTopmostButton = false;

        private ClipboardLineList showingClipboardLineList = new ClipboardLineList();

        private SettingsFileEntry settingsFileEntry;
        private MainWindowSettingsController mainWindowSettingsController;

        public Point NowPositionOnScreen
        {
            get => new Point(Left, Top);
            set
            {
                Left = value.X;
                Top = value.Y;

                mainWindowSettingsController.SetNewPositionOnScreen(value);
            }
        }

        public bool TopmostButtonIsPinned
        {
            get => isPinnedTopmostButton;
            set
            {
                isPinnedTopmostButton = value;
                Topmost = value;
                UpdatePinEdgeVisiblity();

                mainWindowSettingsController.SetTopmostState(value);
            }
        }

        public bool CanBeTransparent
        {
            get => checkboxCanBeTransparent.IsChecked != null && (bool)checkboxCanBeTransparent.IsChecked;
            set
            {
                checkboxCanBeTransparent.IsChecked = value;
                mainWindowSettingsController.SetCanBeTransparent(value);
            }
        }

        public double OpacityAtMouseLeaving
        {
            get => sliderOpacityAtMouseLeaving.Value;
            set
            {
                if (value > 1.0)
                    sliderOpacityAtMouseLeaving.Value = 1.0;
                else if (value < 0)
                    sliderOpacityAtMouseLeaving.Value = 0;
                else
                    sliderOpacityAtMouseLeaving.Value = value;

                mainWindowSettingsController.SetOpacityAtMouseLeaving(value);
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            Loaded += ApplyBeforeSettings;
        }

        public void UpdatePinEdgeVisiblity()
        {
            if (isPinnedTopmostButton)
                PinEdge.Visibility = Visibility.Visible;
            else
                PinEdge.Visibility = Visibility.Hidden;
        }

        private void ApplyBeforeSettings(object sender, RoutedEventArgs e)
        {
            bool isSuccessfulLoadingSettings = LoadBeforeSettings();

            if (!isSuccessfulLoadingSettings)
                return;
            
            ApplyBeforeWindowSettings();

            //TODO : 클립보드 세팅 로드 및 적용
            foreach (ClipboardLinePanel nowClipboardLine in
                showingClipboardLineList.ClipboardLines)
            {
                ClipboardLineListPanel.Children.Add(nowClipboardLine);
                ClipboardLineListPanel.MouseDown += (object _, MouseButtonEventArgs me) =>
                {
                    Debug.WriteLine("Mouse is Over!");
                };
            }
        }

        private bool LoadBeforeSettings()
        {
            UpdatePinEdgeVisiblity();

            try
            {
                settingsFileEntry = new SettingsFileEntry(this);
            }
            catch (IOException e)
            {
                //TODO : 기존 환경설정 파일 읽기 실패시 예외처리
                return false;
            }

            Configuration nowWindowSettings = settingsFileEntry.WindowSettingFromFile;
            mainWindowSettingsController = new MainWindowSettingsController(this, nowWindowSettings);
            bool isSuccessfulLoad = mainWindowSettingsController.IsLoadedWindowSettings;

            return isSuccessfulLoad;
        }

        private void ApplyBeforeWindowSettings()
        {
            if (mainWindowSettingsController.IsLoadedWindowSettings)
            {
                mainWindowSettingsController.DefaultLeftOnScreen = Left;
                mainWindowSettingsController.DefaultTopOnScreen = Top;

                mainWindowSettingsController.ApplyAllCurrentSettings();
            }
        }

        private void IsClickedForDrag(object sender, MouseEventArgs mouseEventArgs)
        {
            if (mouseEventArgs.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
                ApplyLocationToProperty();
            }
        }

        private void IsClickedTopmostButton(object sender, RoutedEventArgs e)
        {
            TopmostButtonIsPinned = !TopmostButtonIsPinned;
        }

        private void IsToggledCheckBoxCanBeTransparent(object sender, RoutedEventArgs e)
        {
            if(checkboxCanBeTransparent.IsChecked != null)
                CanBeTransparent = (bool)checkboxCanBeTransparent.IsChecked;
        }

        private void IsMouseLeave(object sender, RoutedEventArgs e)
        {
            if (checkboxCanBeTransparent.IsChecked.GetValueOrDefault())
                Opacity = sliderOpacityAtMouseLeaving.Value;
        }

        private void IsMouseEnter(object sender, RoutedEventArgs e)
        {
            Opacity = 1;
        }

        private void IsChangedOpacitySlide(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke((Action)(() =>
            {
                if (
                    checkboxCanBeTransparent?.IsChecked != null &&
                    (bool)checkboxCanBeTransparent.IsChecked
                    )
                {
                    if (CheckIsMouseOverAtThisMoment())
                        Opacity = 1;
                    else
                        Opacity = sliderOpacityAtMouseLeaving.Value;
                }
            }));
        }

        private void IsFixedOpacitySlide(object sender, RoutedEventArgs e)
        {
            OpacityAtMouseLeaving = sliderOpacityAtMouseLeaving.Value;
        }

        private void IsFixedOpacitySlideByKey(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Left || e.Key == Key.Right)
                OpacityAtMouseLeaving = sliderOpacityAtMouseLeaving.Value;
        }

        private void NotifyIsChangedLocationOnScreen(object sender, EventArgs changedEvent) => isChangedLocationOnScreen = true;

        private void ApplyLocationToProperty()
        {
            if (isChangedLocationOnScreen)
            {
                NowPositionOnScreen = new Point(Left, Top);

                isChangedLocationOnScreen = false;
            }
        }

        private bool CheckIsMouseOverAtThisMoment()
        {
            var mousePosition = Mouse.GetPosition(this);
            return
                mousePosition.X >= 0 && mousePosition.X <= Width &&
                mousePosition.Y >= 0 && mousePosition.Y <= Height;
        }

#if DEBUG
        private void IsMouseTest(object sender, MouseEventArgs e)
        {
            Console.WriteLine(e.GetPosition(this));
        }
#endif

        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = true;

        private void CommandBinding_CloseWindow(object sender, ExecutedRoutedEventArgs e) => SystemCommands.CloseWindow(this);

        private void CommandBinding_MinimizeWindow(object sender, ExecutedRoutedEventArgs e) => SystemCommands.MinimizeWindow(this);
    }
}
