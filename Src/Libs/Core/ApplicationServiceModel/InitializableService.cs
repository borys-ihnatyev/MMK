using System;
using System.Diagnostics.Contracts;

namespace MMK.ApplicationServiceModel
{
    public abstract class InitializableService : IService
    {
        public bool IsStarted { get; private set; }
        public bool IsInitialized { get; private set; }

        public void Initialize()
        {
            if (IsInitialized)
                return;

            OnInitialize();

            IsInitialized = true;
            OnInitialized();
        }

        protected virtual void OnInitialize()
        {

        }

        public void Start()
        {
            if(IsStarted)
                return;

            OnStart();
            IsStarted = true;
        }

        [ContractInvariantMethod]
        protected void CheckInitialized()
        {
            if (!IsInitialized)
                throw new InvalidOperationException("InitializableService is not initialized");
            Contract.EndContractBlock();
        }

        protected abstract void OnStart();

        public void Stop()
        {
            if(!IsStarted)
                return;
            OnStop();
            IsStarted = false;
        }

        protected abstract void OnStop();

        public event EventHandler Initialized;

        protected virtual void OnInitialized()
        {
            var handler = Initialized;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }
    }
}