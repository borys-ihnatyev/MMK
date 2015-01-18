using System.Windows;

namespace MMK.Presentation.Windows.Input
{
    public interface IGlobalShortcutProvider
    {
        bool IsListening { get; }

        void SetWindow(Window window);

        void StartListen();

        void StopListen();
    }
}
