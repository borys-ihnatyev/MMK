using System;
using System.Timers;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media.Animation;
using MMK.Notify.ViewModel;
using MMK.Wpf.Windows;
using Timer = System.Timers.Timer;

namespace MMK.Notify.View
{
    public partial class BalloonTipWindow
    {
        private Storyboard moveUpStoryBoard;

        private readonly Timer hideTimer;

        public const int DefaultHeight = 120;

        private readonly Taskbar taskbar;

        public BalloonTipWindow()
            : this(new BalloonTipViewModel())
        {
        }

        public BalloonTipWindow(BalloonTipViewModel viewModel)
        {
            InitializeComponent();

            Hide();

            taskbar = new Taskbar();

            hideTimer = new Timer(2000) {AutoReset = true};
            hideTimer.Elapsed += ShowTimeElapsed;

            DataContext = viewModel;
            SizeChanged += BalloonTipWindow_SizeChanged;
            Closing += BalloonTipWindow_Closing;
            MouseDown += (s, e) => Close();
        }

        private void BalloonTipWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            hideTimer.Stop();
            hideTimer.Dispose();
        }

        private void BalloonTipWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            switch (taskbar.Position)
            {
                case Taskbar.TaskbarPosition.Bottom:
                case Taskbar.TaskbarPosition.Right:
                case Taskbar.TaskbarPosition.Top:
                    Left = Screen.PrimaryScreen.WorkingArea.Right - e.NewSize.Width - 5;
                    break;
                case Taskbar.TaskbarPosition.Left:
                    Left = Screen.PrimaryScreen.WorkingArea.Left + 5;
                    break;
            }
        }

        private void ShowTimeElapsed(object sender, ElapsedEventArgs e)
        {
            App.Current.Dispatcher.Invoke(
                () => RaiseEvent(new RoutedEventArgs(HideBeginEvent))
            );
        }

        private void HideCompleted(object sender, EventArgs e)
        {
            Close();
        }

        public new void Show()
        {
            SetStartupPosition();
            base.Show();
            hideTimer.Start();
        }

        private void SetStartupPosition()
        {
            switch (taskbar.Position)
            {
                case Taskbar.TaskbarPosition.Left:
                case Taskbar.TaskbarPosition.Bottom:
                case Taskbar.TaskbarPosition.Right:
                    Top = Screen.PrimaryScreen.WorkingArea.Bottom - Height - 7;
                    break;
                case Taskbar.TaskbarPosition.Top:
                    Top = Screen.PrimaryScreen.WorkingArea.Top + 7;
                    break;
            }
        }

        private void AddMove()
        {

            var animation = new DoubleAnimation(
                CalcMoveNewTop(),
                new Duration(TimeSpan.FromMilliseconds(500))
            );

            Storyboard.SetTarget(animation, this);
            Storyboard.SetTargetProperty(animation, new PropertyPath(TopProperty));

            moveUpStoryBoard = new Storyboard
            {
                Children = new TimelineCollection(new[] {animation})
            };

            moveUpStoryBoard.Completed += (s, e) => moveUpStoryBoard = null;
            moveUpStoryBoard.Begin();
        }

        private double CalcMoveNewTop()
        {
            if(taskbar.Position == Taskbar.TaskbarPosition.Top)
                return Top + (Height + 5);
            return Top - (Height + 5);
        }

        public void Move()
        {
            if (moveUpStoryBoard == null)
                AddMove();
            else
                moveUpStoryBoard.Completed += (s, e) => AddMove();
        }

        public static RoutedEvent HideBeginEvent = EventManager.RegisterRoutedEvent(
            "HideBegin", RoutingStrategy.Direct, typeof (RoutedEventHandler), typeof (BalloonTipWindow));

        public event RoutedEventHandler HideBegin
        {
            add { AddHandler(HideBeginEvent, value); }
            remove { RemoveHandler(HideBeginEvent, value); }
        }
    }
}