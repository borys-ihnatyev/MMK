namespace MMK.Notify.Observer.Tasking.Observing
{
    public partial class TaskObserver
    {
        public class TaskQueuedEventArgs : EventArgs
        {
            internal TaskQueuedEventArgs(TaskObserver observer, int taskCount)
                : base(observer)
            {
                TaskCount = taskCount;
            }

            public int TaskCount { get; private set; }
        }
    }
}