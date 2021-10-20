using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace OneClickCopy
{
    public partial class ToastNotifier : Border
    {
        private int messagePreservingMilliSec = 3000;
        private DispatcherTimer showTimer = new DispatcherTimer();

        private Storyboard nowPlayingStoryboard = null;
        
        private bool isCloseNotificationButtonClicked = false;

        private bool messageLaunchingLock = true;

        public bool IsLocked { get => messageLaunchingLock; }

        public ToastNotifier()
        {
            InitializeComponent();

            showTimer.Interval = new TimeSpan(0, 0, 0, 0, messagePreservingMilliSec);
            showTimer.Tick += new EventHandler(OutTheMessage);
        }

        public static void TryToLaunchMessage(string message)
        {
            ToastNotifier messageNotifier = GetMessageNotifierFromMainWindow();

            messageNotifier?.LaunchTheMessage(message);
        }

        public static ToastNotifier GetMessageNotifierFromMainWindow()
        {
            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;

            return mainWindow?.messageNotifier;
        }

        public void DisableLock() => messageLaunchingLock = false;
        public void EnableLock() => messageLaunchingLock = true;

        public void LaunchTheMessage(string message)
        {
            if (IsLocked)
                return;

            InitializeNotifier();
            messageTextBlock.Text = message;
            Visibility = Visibility.Visible;

            Storyboard storyboardFadeIn = (Storyboard)(Resources["StoryboardFadeIn"]);

            BeginStoryboard(storyboardFadeIn);
        }

        private void InitializeNotifier()
        {
            if (showTimer.IsEnabled)
                showTimer.Stop();

            if (nowPlayingStoryboard != null)
            {
                nowPlayingStoryboard.Stop();
                nowPlayingStoryboard = null;
            }

            isCloseNotificationButtonClicked = false;
        }

        private void PreserveTheMessage(object sender, EventArgs e)
        {
            if (isCloseNotificationButtonClicked)
                return;

            if (nowPlayingStoryboard != null)
            {
                nowPlayingStoryboard.Stop(this);
                nowPlayingStoryboard = null;
            }

            if (showTimer.IsEnabled)
                showTimer.Stop();

            Opacity = 1;
        }

        private void OutTheMessage(object sender, EventArgs e)
        {
            if (showTimer.IsEnabled)
                showTimer.Stop();

            if (!isCloseNotificationButtonClicked)
                FadeOut();

            if (sender == closeNotificationButton)
                isCloseNotificationButtonClicked = true;
        }

        private void FadeOut()
        {
            Storyboard storyboardFadeOut = (Storyboard)Resources["StoryboardFadeOut"];

            BeginStoryboard(storyboardFadeOut);

            Opacity = 0;
        }

        private void WaitAndOut(object sender, EventArgs e)
        {
            Opacity = 1;

            if (!IsMouseOver)
                showTimer.Start();
        }

        private void MakeHidden(object sender, EventArgs e)
        {
            Visibility = Visibility.Hidden;
        }

        private new void BeginStoryboard(Storyboard storyboard)
        {
            if (nowPlayingStoryboard != null)
                nowPlayingStoryboard.Remove(this);

            storyboard.Completed -= ReportPlayingEnded;
            storyboard.Completed += ReportPlayingEnded;

            storyboard.Begin(this, true);
            nowPlayingStoryboard = storyboard;
        }

        private void ReportPlayingEnded(object sender, EventArgs e)
            => nowPlayingStoryboard = null;
    }
}
