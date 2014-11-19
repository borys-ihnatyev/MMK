using System;

namespace MMK.Notify.Observer
{
    [Serializable]
    public class NotifyMessage : INotifyable
    {
        public NotifyType Type { get; set; }
        public string CommonDescription { get; set; }
        public string DetailedDescription { get; set; }
        public string TargetObject { get; set; }
    }
}
