namespace MMK.Application
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
        }

        protected virtual void OnInitialize()
        {

        }

        public abstract void Start();
        public abstract void Stop();
    }
}