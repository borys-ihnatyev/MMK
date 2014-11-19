using System;
using System.Diagnostics.Contracts;

namespace MMK.Notify.Observer
{
    [Serializable]
    public class NotifyableException : Exception, INotifyable
    {
        internal NotifyableException()
        {
            
        }

        public NotifyableException(Exception innerException):base(innerException.Message,innerException)
        {
            Contract.Requires(innerException != null);
            CanContinue = false;
            DetailedDescription = innerException.Message;
        }

        public bool CanContinue { get; set; }
        public NotifyType Type { get { return NotifyType.Error; } }
        public string CommonDescription { get; set; }
        public string DetailedDescription { get; set; }
        public string TargetObject { get; set; }
    }
}
