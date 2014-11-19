using System;
using System.Diagnostics.Contracts;
using System.Runtime.Serialization;

namespace MMK.Notify.Observer.Tasking
{
    [Serializable]
    public abstract partial class Task : INotifyable, ISerializable
    {
        protected Task()
        {
            RunCount = 0;
        }

        public int RunCount { get; private set; }

        public ObservedInfo Run()
        {
            Contract.Ensures(Contract.Result<ObservedInfo>() != null);
            try
            {
                ++RunCount;
                TryInitialize();
                OnRun();
                return new ObservedInfo(this);
            }
            catch (NotifyableException ex)
            {
                return new ObservedInfo(ex) {
                    Task =  this
                };
            }
        }

        protected abstract void OnRun();

        private void TryInitialize()
        {
            try
            {
                Initialize();
            }
            catch (Cancel)
            {
                throw;
            }
            catch (NotifyableException)
            {
                throw;
            }
            catch (Exception ex)
            {
                OnInitializeException(ex);
            }
        }

        protected virtual void Initialize()
        {
        }

        private void OnInitializeException(Exception ex)
        {
            Contract.Requires(ex != null);
#if DEBUG
            throw ex;
#else
            ThrowAsNotifyableException(ex);
#endif
        }

        protected void ThrowAsNotifyableException(Exception ex, bool canContinue = false)
        {
            throw new NotifyableException
            {
                CanContinue = canContinue,
                TargetObject = TargetObject,
                CommonDescription = "Что-то пошло не так.",
                DetailedDescription = ex.Message
            };
        }

        #region INotifyable


        protected abstract string CommonDescription
        {
            get;
        }

        protected abstract string DetailedDescription
        {
            get;
        }

        protected abstract string TargetObject
        {
            get;
        }

        string INotifyable.CommonDescription
        {
            get { return CommonDescription; }
        }

        string INotifyable.DetailedDescription
        {
            get { return DetailedDescription; }
        }

        string INotifyable.TargetObject
        {
            get { return TargetObject; }
        }

        NotifyType INotifyable.Type { get { return NotifyType.Success; } }

        #endregion

        #region Serialization

        protected Task(SerializationInfo info, StreamingContext context)
        {
            if(info == null)
                throw new ArgumentNullException("info");
            Contract.EndContractBlock();

            RunCount = 0;
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException("info");
            Contract.EndContractBlock();

        }

        #endregion
    }
}
