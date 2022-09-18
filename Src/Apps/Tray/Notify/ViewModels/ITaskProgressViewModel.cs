using MMK.Notify.Observer;

namespace MMK.Notify.ViewModels
{
    public interface ITaskProgressViewModel
    {
        INotifyable CurrentInfo { get;}
        bool IsProgress { get; }
        int QueuedCount { get; }
        int ObservedCount { get; }
        int FailedCount { get; }
    }
}