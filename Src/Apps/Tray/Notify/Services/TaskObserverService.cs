using MMK.ApplicationServiceModel;
using MMK.Notify.Observer.Remoting;
using MMK.Notify.Observer.Tasking.Observing;

namespace MMK.Notify.Services
{
    public class TaskObserverService : Service
    {
        private readonly NotificationService notification;
        private readonly TaskObserver observer;
        private readonly NotifyObserver notifyObserver;

        public TaskObserverService(
            NotificationService notification, 
            TaskObserver observer,
            NotifyObserver notifyObserver)
        {
            this.notification = notification;
            this.observer = observer;
            this.notifyObserver = notifyObserver;
        }

        public override void Start()
        {
            BindEvents();
            notifyObserver.Start();
            observer.Start();
        }

        private void BindEvents()
        {
            observer.TaskObserved += ObserverOnTaskObserved;
        }

        public override void Stop()
        {
            UnbindEvents();
            observer.Cancell();
            notifyObserver.Stop();
        }

        private void UnbindEvents()
        {
            observer.TaskObserved -= ObserverOnTaskObserved;
        }

        private void ObserverOnTaskObserved(object sender, TaskObserver.NotifyEventArgs e)
        {
            notification.Push(e.Message);
        }
    }
}