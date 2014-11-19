using System;
using System.Diagnostics.Contracts;

namespace MMK.Notify.Observer.Tasking.Common
{
    [Serializable]
    public class NotifyTask : Task
    {
        private readonly INotifyable notifyable;

        public NotifyTask(INotifyable notifyable)
        {
            if(notifyable == null)
                throw new ArgumentNullException("notifyable");
            Contract.EndContractBlock();

            this.notifyable = notifyable;
        }

        protected override void OnRun()
        {
            var exception = notifyable as NotifyableException;
            
            if (exception != null)
                throw exception;
        }

        protected override string CommonDescription
        {
            get { return notifyable.CommonDescription; }
        }

        protected override string DetailedDescription
        {
            get { return notifyable.DetailedDescription; }
        }

        protected override string TargetObject
        {
            get { return notifyable.TargetObject; }
        }
    }
}
