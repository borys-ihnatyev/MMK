namespace MMK.Notify.Observer.Tasking.Observing
{
    public partial class TaskObserver
    {
        public class EventArgs : System.EventArgs
        {
            internal EventArgs(TaskObserver observer)
            {
                QueuedTaskCount = observer.tasks.Count;
            }

            public int QueuedTaskCount { get; private set; }
        }
    }
}