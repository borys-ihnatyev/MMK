using MMK.Notify.Observer;
using MMK.Notify.ViewModels;

namespace MMK.Notify.DesignData
{
    public class TaskProgressViewModel : ITaskProgressViewModel
    {
        public INotifyable CurrentInfo { get; set; }

        public bool IsProgress { get; set; }

        public int QueuedCount { get; set; }

        public int ObservedCount { get; set; }

        public int FailedCount { get; set; }
    }
}