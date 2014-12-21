namespace MMK.Notify.Observer.Tasking.Observing
{
    public partial class TaskObserver
    {
        public class NotifyEventArgs : EventArgs
        {
            internal NotifyEventArgs(TaskObserver observer, INotifyable message)
                : base(observer)
            {
                Message = message;
            }

            public INotifyable Message { get; private set; }
        }
    }
}