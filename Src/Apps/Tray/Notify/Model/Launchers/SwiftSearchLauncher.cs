using System.Windows;
using System.Windows.Forms;
using MMK.SwiftSearch;
using MMK.Wpf;
using MMK.Wpf.Providers;
using Clipboard = System.Windows.Clipboard;

namespace MMK.Notify.Model.Launchers
{
    sealed public class SwiftSearchLauncher : GlobalShortcutProviderCollection
    {
        private readonly WindowLauncher<SearchWindow> launcher = new WindowLauncher<SearchWindow>();

        private bool isSetStartShortcut;

        private bool isSetStartFromClipboardShortcut;

        public SwiftSearchLauncher()
        {

        }

        public SwiftSearchLauncher(Window window):base(window)
        {
        }

        public void SetStartShortcut(KeyModifyers modifyers, Keys key)
        {
            if(isSetStartShortcut) return;

            isSetStartShortcut = Add(modifyers, (int)key, Start);
        }

        private void Start()
        {
            App.Current.StopListenShortcuts();
            launcher.Launch();
            launcher.Window.Closed += (s, e) => App.Current.StartListenShortcuts();
        }

        public void SetStartFromClipboardShortcut(KeyModifyers modifyers, Keys key)
        {
            if(isSetStartFromClipboardShortcut)return;

            isSetStartFromClipboardShortcut = Add(modifyers, (int)key, StartFromClipboard);
        }

        private void StartFromClipboard()
        {
            var search = string.Empty;
            if (Clipboard.ContainsText())
                search = Clipboard.GetText();

            Start();
            launcher.Window.Search = search;
        }
    }
}
