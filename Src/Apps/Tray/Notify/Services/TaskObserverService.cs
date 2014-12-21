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

        public TaskObserverService(NotificationService notification, TaskObserver observer,
            NotifyObserver notifyObserver)
        {
            this.notification = notification;
            this.observer = observer;
            this.notifyObserver = notifyObserver;
        }

        private void ObserverOnTaskObserved(object sender, TaskObserver.NotifyEventArgs e)
        {
            notification.Push(e.Message);
        }

        public override void Start()
        {
            observer.TaskObserved += ObserverOnTaskObserved;
            notifyObserver.Start();
            observer.Start();
        }

        public override void Stop()
        {
            observer.TaskObserved -= ObserverOnTaskObserved;
            observer.Cancell();
            notifyObserver.Stop();
        }
    }
}