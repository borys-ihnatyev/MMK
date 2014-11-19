namespace MMK.Notify.Observer
{
    public interface INotifyable
    {
        NotifyType Type { get; }
        string CommonDescription { get; }
        string DetailedDescription { get; }
        string TargetObject { get; }
    }
}
