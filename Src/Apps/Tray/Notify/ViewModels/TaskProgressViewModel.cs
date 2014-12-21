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
            var taskObserver = IoC.ServiceLocator.Get<TaskObserver>();
            taskObserver.TaskQueued += TaskObserverOnTaskQueued;
            taskObserver.QueueEmpty += OnQueueEmpty;
            taskObserver.TaskObserved += OnTaskObserved;
        }

        protected override void OnUnloadData()
        {
            var taskObserver = IoC.ServiceLocator.Get<TaskObserver>();

            taskObserver.TaskQueued -= TaskObserverOnTaskQueued;
            taskObserver.QueueEmpty -= OnQueueEmpty;
            taskObserver.TaskObserved -= OnTaskObserved;
        }

        private void TaskObserverOnTaskQueued(object sender, TaskObserver.TaskQueuedEventArgs e)
        {
            QueuedCount += e.TaskCount;
        }

        private void OnTaskObserved(object sender, TaskObserver.NotifyEventArgs e)
        {
            ++ObservedCount;
            CurrentInfo = e.Message;
        }

        private void OnQueueEmpty(object sender, EventArgs e)
        {
            QueuedCount = 0;
            ObservedCount = 0;
            currentInfo = null;
        }
    }
}