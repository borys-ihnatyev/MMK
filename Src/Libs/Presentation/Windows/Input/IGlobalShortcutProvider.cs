namespace MMK.Presentation.Windows.Input
{
    public interface IGlobalShortcutProvider
    {
        bool IsListening { get; }

        void StartListen();

        void StopListen();
    }
}