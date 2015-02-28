using System;
using System.Diagnostics.Contracts;

namespace MMK.Notify.Observer.Tasking
{
    public abstract partial class Task
    {
        private int runCount;
        private ITaskContext context;

        protected Task(ITaskContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            Contract.EndContractBlock();

            Context = context;
        }

        protected Task()
        {
        }

        protected internal ITaskContext Context
        {
            get { return context; }
            set
            {
                context = value;
                CheckContext();
            }
        }

        public ObservedInfo Run()
        {
            Contract.Ensures(Contract.Result<ObservedInfo>() != null);
            Contract.EndContractBlock();
            var result = TryRun();
            return ObservedInfo.Build(result, runCount);
        }

        protected virtual void CheckContext()
        {
            if (context == null)
                throw new InvalidTaskContextException("context is not setup");
        }

        private INotifyable TryRun()
        {
            INotifyable result;
            try
            {
                ++runCount;
                result = OnRun();
            }
            catch (NotifyableException ex)
            {
                result = ex;
            }
            return result;
        }

        protected abstract INotifyable OnRun();


        protected NotifyableException NotifyableException(Exception ex, bool canContinue = false)
        {
            return new NotifyableException
            {
                CanContinue = canContinue,
                TargetObject = Context.ToString(),
                CommonDescription = "Что-то пошло не так.",
                DetailedDescription = ex.Message
            };
        }
    }
}