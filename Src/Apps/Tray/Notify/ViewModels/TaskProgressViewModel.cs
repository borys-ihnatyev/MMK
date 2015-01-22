using System;
using System.Diagnostics.Contracts;
using MMK.ApplicationServiceModel;
using MMK.Notify.Observer;
using MMK.Notify.Observer.Tasking.Observing;
using MMK.Notify.Services;
using MMK.Presentation.ViewModel;

namespace MMK.Notify.ViewModels
{
    public class TaskProgressViewModel : ViewModel, ITaskProgressViewModel
    {
        private readonly NotificationService notificationService;

        private INotifyable currentInfo;

        private bool isVisible;
        private bool isProgress;
        private int observedCount;
        private int queuedCount;
        private int failedCount;

        public TaskProgressViewModel(NotificationService notificationService)
        {
            this.notificationService = notificationService;
        }

        public INotifyable CurrentInfo
        {
            get { return currentInfo; }
            private set
            {
                if (value == currentInfo)
                    return;
                currentInfo = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsVisible
        {
            get { return isVisible; }
            set
            {
                if (value == isVisible)
                    return;
                isVisible = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsProgress
        {
            get { return isProgress; }
            private set
            {
                if (value == isProgress)
                    return;

                isProgress = value;

                IsVisible = IsProgress && QueuedCount > 1;

                NotifyPropertyChanged();
            }
        }

        public int QueuedCount
        {
            get { return queuedCount; }
            private set
            {
                if (value < 0)
                    throw new ArgumentException(@"must be >= 0", "value");
                if (value < ObservedCount)
                    throw new ArgumentException(@"must be < ObservedCount", "value");
                Contract.EndContractBlock();

                if (value == queuedCount)
                    return;

                queuedCount = value;
                IsProgress = queuedCount != 0;
                NotifyPropertyChanged();
            }
        }

        public int ObservedCount
        {
            get { return observedCount; }
            private set
            {
                if (value < 0)
                    throw new ArgumentException(@"must be >= 0", "value");
                Contract.EndContractBlock();

                if (value == observedCount)
                    return;
                observedCount = value;
                NotifyPropertyChanged();
            }
        }

        public int FailedCount
        {
            get { return failedCount; }
            private set
            {
                if(value == failedCount)
                    return;

                failedCount = value;
                NotifyPropertyChanged();
            }
        }

        protected override void OnLoadData()
        {
            var observer = IoC.Get<TaskObserver>();
            
            observer.TaskObserved += OnTaskObserved;
            observer.TaskFailed += OnTaskFailed;
            observer.TaskCanceled += OnTaskCanceled;

            observer.TaskQueued += OnTaskQueued;
            observer.QueueEmpty += OnQueueEmpty;
        }

        protected override void OnUnloadData()
        {
            var observer = IoC.Get<TaskObserver>();
            
            observer.TaskObserved -= OnTaskObserved;
            observer.TaskFailed -= OnTaskFailed;
            observer.TaskCanceled -= OnTaskCanceled;
            
            observer.TaskQueued -= OnTaskQueued;
            observer.QueueEmpty -= OnQueueEmpty;
        }


        public void OnTaskObserved(object sender, TaskObserver.NotifyEventArgs e)
        {
            ++ObservedCount;
            CurrentInfo = e.Message;
        }

        private void OnTaskFailed(object sender, TaskObserver.NotifyEventArgs e)
        {
            ++FailedCount;
        }

        private void OnTaskCanceled(object sender, TaskObserver.NotifyEventArgs e)
        {
            --QueuedCount;
        }


        public void OnTaskQueued(object sender, TaskObserver.TaskQueuedEventArgs e)
        {
            QueuedCount += e.TaskCount;
        }

        public void OnQueueEmpty(object sender, EventArgs e)
        {
            Notify();
            Reset();
        }

        private void Notify()
        {
            if (!CanNotify)
                return;

            var message = BuildNotifyMessage();

            notificationService.Push(message);
        }

        private bool CanNotify
        {
            get { return QueuedCount != 0 && ((ObservedCount != 0) || failedCount != 0); }
        }

        private INotifyable BuildNotifyMessage()
        {
            Contract.Ensures(Contract.Result<INotifyable>() != null);
            Contract.EndContractBlock();

            if (QueuedCount == 1)
                if (CurrentInfo != null)
                    return CurrentInfo;

            if (failedCount == 0)
                return new NotifyMessage
                {
                    Type = NotifyType.Success,
                    CommonDescription = "All tasks done.",
                    DetailedDescription = String.Format("{0} Tasks", QueuedCount)
                };

            if (failedCount == ObservedCount)
                return new NotifyMessage
                {
                    Type = NotifyType.Error,
                    CommonDescription = "All tasks failed.",
                    DetailedDescription = String.Format("{0}/{1} Tasks", failedCount, QueuedCount)
                };

            return new NotifyMessage
            {
                Type = NotifyType.Warning,
                CommonDescription = "Some tasks failed.",
                DetailedDescription =
                    String.Format("Done {0}/{1};\nFailed {2}", ObservedCount - failedCount, QueuedCount, failedCount)
            };
        }

        private void Reset()
        {
            ObservedCount = 0;
            QueuedCount = 0;
            failedCount = 0;
            IsVisible = false;
            currentInfo = null;
        }
    }
}