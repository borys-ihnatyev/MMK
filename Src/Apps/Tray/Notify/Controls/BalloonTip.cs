using System.Collections.Generic;
using System.Timers;
using MMK.Notify.Controls.Model;
using MMK.Notify.Controls.View;
using MMK.Notify.Controls.ViewModel;
using MMK.Notify.Observer;
using Timer = System.Timers.Timer;

namespace MMK.Notify.Controls
{
    public class BalloonTip
    {
        private readonly Timer messageQueueTimer;
        private readonly Queue<BalloonTipViewModel> messageQueue;
        private readonly LinkedList<BalloonTipWindow> activeBallonTipWindows;

        public BalloonTip()
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
                if(TryStopTimerOnEmptyQueue())
                    return;
     
                var message = messageQueue.Dequeue();
                App.Current.Dispatcher.Invoke(() => Push(message));

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
                window.MoveUp();
            AddBallonTipWindow(balloonTipWindow);
        }

        private void AddBallonTipWindow(BalloonTipWindow balloonTipWindow)
        {
            activeBallonTipWindows.AddLast(balloonTipWindow);
            balloonTipWindow.Closed += (s, e) => activeBallonTipWindows.Remove(s as BalloonTipWindow);
            balloonTipWindow.Show();
        }

        public void Push(NotifyType type, string title, string details = "", string targetObject = "")
        {
            lock (messageQueue)
            {
                var message = new BalloonTipViewModel
                {
                    NotifyType = type,
                    Title = title,
                    Details = details,
                    TargetObject = targetObject
                };
                messageQueue.Enqueue(message);
                messageQueueTimer.Start();
            }
        }

        public void Push(INotifyable notifyable)
        {
            Push(
                notifyable.Type,
                notifyable.CommonDescription,
                notifyable.DetailedDescription,
                notifyable.TargetObject
            );
        }
    }
}