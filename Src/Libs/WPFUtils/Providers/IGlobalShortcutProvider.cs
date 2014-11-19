using System.Windows;

namespace MMK.Wpf.Providers
{
    public interface IGlobalShortcutProvider
    {
        bool IsListening { get; }

        void SetWindow(Window window);

        void StartListen();

        void StopListen();
    }
}
