using System.Collections.Generic;
using System.Timers;
using System.Windows;
using MMK.Notify.Observer;
using MMK.Notify.ViewModels;
using MMK.Notify.Views;

namespace MMK.Notify.Services
{
    public class NotificationService
    {
        private readonly Timer messageQueueTimer;
        private readonly Queue<BalloonTipViewModel> messageQueue;
        private readonly LinkedList<BalloonTipWindow> activeBallonTipWindows;

        public NotificationService()
        {
            messageQueueTimer = new Timer(700);
            messageQueueTimer.Elapsed += OnTimerTryPush;

            messageQueue = new Queue<BalloonTipViewModel>();
            activeBallonTipWindows = new LinkedList<BalloonTipWindow>();
        }

        private void OnTimerTryPush(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            lock (messageQueue)
            {
                if (TryStopTimerOnEmptyQueue())
                    return;

                var message = messageQueue.Dequeue();
                Application.Current.Dispatcher.Invoke(() => Push(message));

                TryStopTimerOnEmptyQueue();
            }
        }

        private bool TryStopTimerOnEmptyQueue()
        {
            if (messageQueue.Count != 0) return false;

            messageQueueTimer.Stop();
            return true;
        }

        private void Push(BalloonTipViewModel message)
        {
            var balloonTipWindow = new BalloonTipWindow(message);
            foreach (var window in activeBallonTipWindows)
                window.Move();
            AddBallonTipWindow(balloonTipWindow);
        }

        private void AddBallonTipWindow(BalloonTipWindow balloonTipWindow)
        {
            activeBallonTipWindows.AddLast(balloonTipWindow);
            balloonTipWindow.Closed += (s, e) => activeBallonTipWindows.Remove(s as BalloonTipWindow);
            balloonTipWindow.Show();
        }

        public void Push(INotifyable notifyable)
        {
            lock (messageQueue)
            {
                var message = new BalloonTipViewModel(notifyable);
                messageQueue.Enqueue(message);
                messageQueueTimer.Start();
            }
        }
    }
}