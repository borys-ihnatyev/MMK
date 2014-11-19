using MMK.Notify.Controls.ViewModel;
using System;
using System.Timers;
using System.Windows;
using System.Windows.Media.Animation;

namespace MMK.Notify.Controls.View
{
    public partial class BalloonTipWindow
    {
        private Storyboard moveUpStoryBoard;

        private readonly Timer hideTimer;

        public const int DefaultHeight = 120;

        public BalloonTipWindow()
            : this(new BalloonTipViewModel())
        {
        }

        public BalloonTipWindow(BalloonTipViewModel viewModel)
        {
            InitializeComponent();

            Hide();

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
            Left = SystemParameters.WorkArea.Right - e.NewSize.Width - 5;
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
            Top = SystemParameters.WorkArea.Bottom - Height;
            base.Show();
            hideTimer.Start();
        }

        private void AddMoveUp()
        {
            var animation = new DoubleAnimation(
                Top - (Height + 5),
                new Duration(new TimeSpan(0, 0, 0, 0, 500))
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

        public void MoveUp()
        {
            if (moveUpStoryBoard == null)
                AddMoveUp();
            else
                moveUpStoryBoard.Completed += (s, e) => AddMoveUp();
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