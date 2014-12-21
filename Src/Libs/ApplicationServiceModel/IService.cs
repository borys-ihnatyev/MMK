namespace MMK.Application
{
    public interface IService
    {
        bool IsInitialized { get; }

        void Start();
        void Stop();
    }
}