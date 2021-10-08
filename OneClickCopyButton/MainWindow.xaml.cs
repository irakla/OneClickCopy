using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OneClickCopy
{

    public partial class MainWindow : Window
    {
        private bool isPinnedTopmostButton = false;

        private ClipboardLineList showingClipboardLineList = new ClipboardLineList();

        private SettingsFileEntry settingsFileEntry;
        private MainWindowSettingsController mainWindowSettingsController;

        private Storyboard storyboardOpacityFadeInOut = null;
        private DoubleAnimation animationOpacityToThick = null;
        private DoubleAnimation animationOpacityToTransparent = null;

        private ResourceManager messageResourceManager = new ResourceManager(typeof(OneClickCopy.Properties.MessageResource));

        private bool IsReadyMainWindowSettingController
        {
            get => mainWindowSettingsController != null && mainWindowSettingsController.IsLoadedWindowSettings;
        }

        public Point NowPositionOnScreen
        {
            get => new Point(Left, Top);
            set
            {
                double availableLeftBound = SystemParameters.VirtualScreenLeft - (Width != double.NaN ? Width : 0);
                double availableTopBound = SystemParameters.VirtualScreenTop - (TitleArea.Height != double.NaN ? TitleArea.Height : 0);
                double availableRightBound = SystemParameters.VirtualScreenWidth;
                double availableBottomBound = SystemParameters.VirtualScreenHeight;

                if (value.X <= availableLeftBound || value.X >= availableRightBound ||
                    value.Y <= availableTopBound || value.Y >= availableBottomBound)
                    return;

                Left = value.X;
                Top = value.Y;

                if (IsReadyMainWindowSettingController)
                    mainWindowSettingsController.SetNewPositionOnScreen(value);
            }
        }

        public Size NowWindowSize
        {
            get => new Size(Width, Height);
            set
            {
                Width = value.Width;
                Height = value.Height;

                if (IsReadyMainWindowSettingController)
                    mainWindowSettingsController.SetNewWindowSize(value);
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

                if (IsReadyMainWindowSettingController)
                    mainWindowSettingsController.SetTopmostState(value);
            }
        }

        public bool CanBeTransparent
        {
            get => checkboxCanBeTransparent.IsChecked != null && (bool)checkboxCanBeTransparent.IsChecked;
            set
            {
                checkboxCanBeTransparent.IsChecked = value;

                if (IsReadyMainWindowSettingController)
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
                else if (value < sliderOpacityAtMouseLeaving.Minimum)
                    sliderOpacityAtMouseLeaving.Value = sliderOpacityAtMouseLeaving.Minimum;
                else
                    sliderOpacityAtMouseLeaving.Value = value;

                if(animationOpacityToTransparent != null)
                    animationOpacityToTransparent.To = value;
                if (IsReadyMainWindowSettingController)
                    mainWindowSettingsController.SetOpacityAtMouseLeaving(value);
            }
        }

        public int MilliDurationFullOpacityAnimation
        {
            get;
            private set;
        } = 250;

        public MainWindow()
        {
            InitializeComponent();

            Loaded += ApplyBeforeSettings;
            Loaded += SetWindowConstraintsWithElements;

            Closed += MakePersistentCopy;
        }

        public void LaunchToastNotification(string message)
            => MessageNotifier.LaunchTheMessage(message);

        private void UpdatePinEdgeVisiblity()
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
            foreach (OwnCopyLinePanel nowClipboardLine in
                showingClipboardLineList.ClipboardLines)
            {
                ClipboardLineListPanel.Children.Add(nowClipboardLine);
            }

            if (ClipboardLineListPanel.Children.Count > 0 &&
                ClipboardLineListPanel.Children[0] is FrameworkElement)
            {
                double calculatedMinWidth = ((FrameworkElement)ClipboardLineListPanel.Children[0]).MinWidth;
                ClipboardLineListPanel.MinWidth = calculatedMinWidth;
                MinWidth = calculatedMinWidth;
            }

            InitializeFadeInOutAnimation();
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
                mainWindowSettingsController.DefaultWindowWidth = Width;
                mainWindowSettingsController.DefaultWindowHeight = Height;

                mainWindowSettingsController.ApplyAllCurrentSettings();
            }

            MessageNotifier.DisableLock();
        }

        private void InitializeFadeInOutAnimation()
        {
            storyboardOpacityFadeInOut = new Storyboard();

            animationOpacityToThick = new DoubleAnimation();
            animationOpacityToThick.To = 1;
            animationOpacityToThick.FillBehavior = FillBehavior.HoldEnd;

            animationOpacityToTransparent = new DoubleAnimation();
            animationOpacityToTransparent.To = OpacityAtMouseLeaving;
            animationOpacityToTransparent.FillBehavior = FillBehavior.HoldEnd;

            Storyboard.SetTargetName(animationOpacityToThick, Name);
            Storyboard.SetTargetProperty(animationOpacityToThick, new PropertyPath(Window.OpacityProperty));
            Storyboard.SetTargetName(animationOpacityToTransparent, Name);
            Storyboard.SetTargetProperty(animationOpacityToTransparent, new PropertyPath(Window.OpacityProperty));
        }

        private void SetWindowConstraintsWithElements(object sender, RoutedEventArgs e)
        {
            MinWidth = CustomTitleBar.ActualWidth;
        }

        private void IsClickedForDrag(object sender, MouseEventArgs mouseEventArgs)
        {
            if (mouseEventArgs.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void IsClickedTopmostButton(object sender, RoutedEventArgs e)
        {
            TopmostButtonIsPinned = !TopmostButtonIsPinned;

            if (TopmostButtonIsPinned)
                LaunchToastNotification(messageResourceManager.GetString("WindowTopmostSet"));
            else
                LaunchToastNotification(messageResourceManager.GetString("WindowTopmostUnset"));
        }

        private void IsToggledCheckBoxCanBeTransparent(object sender, RoutedEventArgs e)
        {
            if(checkboxCanBeTransparent.IsChecked != null)
                CanBeTransparent = (bool)checkboxCanBeTransparent.IsChecked;

            if (CanBeTransparent)
                LaunchToastNotification(messageResourceManager.GetString("WindowCanBeTransparent"));
            else
                LaunchToastNotification(messageResourceManager.GetString("WindowCannotBeTransparent"));
        }

        private void MakeWindowTransparent(object sender, MouseEventArgs e)
        {
            if (!CanBeTransparent)
                return;

            if (!IsMouseOverAtThisMoment())
            {
                if(storyboardOpacityFadeInOut != null && OpacityAtMouseLeaving != 1)
                {
                    int milliDurationToTransparent = (int)(MilliDurationFullOpacityAnimation * (Opacity - OpacityAtMouseLeaving) / (1 - OpacityAtMouseLeaving));
                    animationOpacityToTransparent.Duration = new Duration(TimeSpan.FromMilliseconds(milliDurationToTransparent));
                    
                    storyboardOpacityFadeInOut.Children.Clear();
                    storyboardOpacityFadeInOut.Children.Add(animationOpacityToTransparent);
                    storyboardOpacityFadeInOut.Begin(this, true);
                }
            }
        }
        
        private void MakeWindowThick(object sender, RoutedEventArgs e)
        {
            if (storyboardOpacityFadeInOut != null && OpacityAtMouseLeaving != 1)
            {
                int milliDurationToThick = (int)(MilliDurationFullOpacityAnimation * (1 - Opacity) / (1 - OpacityAtMouseLeaving));
                animationOpacityToThick.Duration = new Duration(TimeSpan.FromMilliseconds(milliDurationToThick));

                storyboardOpacityFadeInOut.Children.Clear();
                storyboardOpacityFadeInOut.Children.Add(animationOpacityToThick);
                storyboardOpacityFadeInOut.Begin(this, true);
            }
        }

        private void IsChangedOpacitySlide(object sender, RoutedEventArgs e)
        {
            storyboardOpacityFadeInOut?.Remove(this);

            Dispatcher.BeginInvoke((Action)(() =>
            {
                Opacity = sliderOpacityAtMouseLeaving.Value;
            }));
        }

        private void IsFixedOpacitySlide(object sender, RoutedEventArgs e)
        {
            OpacityAtMouseLeaving = sliderOpacityAtMouseLeaving.Value;

            if (CanBeTransparent && !IsMouseOverAtThisMoment())
                Opacity = OpacityAtMouseLeaving;
            else
                Opacity = 1;

            LaunchToastNotification(GetNowOpacityAtMouseLeavingMessage());
        }

        private void IsFixedOpacitySlideByKey(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left || e.Key == Key.Right || e.Key == Key.Up || e.Key == Key.Down)
            {
                OpacityAtMouseLeaving = sliderOpacityAtMouseLeaving.Value;

                if (IsMouseOverAtThisMoment())
                    Opacity = 1;
            }

            LaunchToastNotification(GetNowOpacityAtMouseLeavingMessage());
        }

        private string GetNowOpacityAtMouseLeavingMessage()
            => messageResourceManager.GetString("WindowOpacityAtMouseLeaving") + " : " +
            string.Format("{0:0.00}", OpacityAtMouseLeaving);

        private void NotifyIsChangedPositionOnScreen(object sender, EventArgs _)
        {
            ApplyNowLocationToProperty();
        }

        private void NotifyIsChangedWindowSize(object sender, EventArgs _)
        {
            ApplySizeToProperty();
        }

        private void ApplyNowLocationToProperty() => NowPositionOnScreen = new Point(Left, Top);
        private void ApplySizeToProperty() => NowWindowSize = new Size(Width, Height);

        [StructLayout(LayoutKind.Sequential)]
        public struct POINTWin32 { public int X, Y; }

        //For tracing Cursor on outside our Window
        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out POINTWin32 lpPoint);

        private bool IsMouseOverAtThisMoment()
        {
            int WindowBorderLeft = (int)(Left + NowWindowChrome.ResizeBorderThickness.Left);
            int WindowBorderRight = (int)(Left + Width - NowWindowChrome.ResizeBorderThickness.Right);
            int WindowBorderTop = (int)(Top + NowWindowChrome.ResizeBorderThickness.Top);
            int WindowBorderBottom = (int)(Top + Height - NowWindowChrome.ResizeBorderThickness.Bottom);
            
            POINTWin32 cursorPosition;
            
            GetCursorPos(out cursorPosition);
            bool isMouseOver =
                cursorPosition.X >= WindowBorderLeft && cursorPosition.X < WindowBorderRight &&
                cursorPosition.Y >= WindowBorderTop && cursorPosition.Y < WindowBorderBottom;

            return isMouseOver;
        }

        private void MakePersistentCopy(object sender, EventArgs e)
        {
            IDataObject latestCopy = showingClipboardLineList.LatestCopy;

            if (latestCopy != null && Clipboard.IsCurrent(latestCopy))
            {
                Clipboard.SetDataObject(latestCopy, true);
            }
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
