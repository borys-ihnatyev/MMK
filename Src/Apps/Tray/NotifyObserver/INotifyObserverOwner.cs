namespace MMK.Notify.Observer
{
    public interface INotifyObserverOwner
    {
        INotifyObserver NotifyObserver { get; }
    }
}
