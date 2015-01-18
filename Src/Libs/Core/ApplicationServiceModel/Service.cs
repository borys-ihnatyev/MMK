using System;
using System.Diagnostics.Contracts;

namespace MMK.ApplicationServiceModel
{
    public abstract class Service : IService
    {
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

        [ContractInvariantMethod]
        protected void CheckInitialized()
        {
            if (!IsInitialized)
                throw new InvalidOperationException("Service is not initialized");
            Contract.EndContractBlock();
        }

        public abstract void Start();
        public abstract void Stop();

        public event EventHandler Initialized;

        protected virtual void OnInitialized()
        {
            var handler = Initialized;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }
    }
}