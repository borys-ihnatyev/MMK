using System;
using System.Diagnostics.Contracts;
using MMK.ApplicationServiceModel;
using MMK.Notify.Observer;
using MMK.Notify.Observer.Tasking.Observing;
using MMK.Wpf.ViewModel;

namespace MMK.Notify.ViewModels
{
    public class TaskProgressViewModel : ViewModel
    {
        private INotifyable currentInfo;
        
        private bool isActive;
        private int observedCount;
        private int queuedCount;

        public INotifyable CurrentInfo
        {
            get { return currentInfo; }
            set
            {
                currentInfo = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsActive
        {
            get { return isActive; }
            private set
            {
                isActive = value;
                NotifyPropertyChanged();                
            }
        }

        public int ObservedCount
        {
            get { return observedCount; }
            set
            {
                if (value < 0)
                    throw new ArgumentException(@"must be >= 0", "value");
                Contract.EndContractBlock();

                if(value == observedCount)
                    return;
                observedCount = value;
                NotifyPropertyChanged();
            }
        }

        public int QueuedCount
        {
            get { return queuedCount; }
            set
            {
                if(value < 0)
                    throw new ArgumentException(@"must be >= 0","value");
                if (value < ObservedCount)
                    throw new ArgumentException(@"must be < ObservedCount", "value");
                Contract.EndContractBlock();

                if(value == queuedCount)
                    return;

                queuedCount = value;
                IsActive = queuedCount != 0;
                NotifyPropertyChanged();
            }
        }

        protected override void OnLoadData()
        {
            var observer = IoC.ServiceLocator.Get<TaskObserver>();
            observer.TaskQueued += OnTaskQueued;
            observer.QueueEmpty += OnQueueEmpty;
            observer.TaskObserved += OnTaskObserved;
        }

        protected override void OnUnloadData()
        {
            var observer = IoC.ServiceLocator.Get<TaskObserver>();
            observer.TaskQueued -= OnTaskQueued;
            observer.QueueEmpty -= OnQueueEmpty;
            observer.TaskObserved -= OnTaskObserved;
        }

        public void OnTaskQueued(object sender, TaskObserver.TaskQueuedEventArgs e)
        {
            QueuedCount += e.TaskCount;
        }

        public void OnTaskObserved(object sender, TaskObserver.NotifyEventArgs e)
        {
            ++ObservedCount;
            CurrentInfo = e.Message;
        }

        public void OnQueueEmpty(object sender, EventArgs e)
        {
            ObservedCount = 0;
            QueuedCount = 0;
            currentInfo = null;
        }
    }
}