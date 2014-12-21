using System.Windows;
using System.Windows.Forms;
using MMK.ApplicationServiceModel;
using MMK.Notify.Services;
using MMK.SwiftSearch;
using MMK.Wpf;
using MMK.Wpf.Providers;
using Clipboard = System.Windows.Clipboard;

namespace MMK.Notify.Model.Launchers
{
    public sealed class SwiftSearchLauncher : GlobalShortcutProviderCollection
    {
        private readonly WindowLauncher<SearchWindow> launcher = new WindowLauncher<SearchWindow>();

        private bool isSetStartShortcut;

        private bool isSetStartFromClipboardShortcut;

        public SwiftSearchLauncher()
        {
        }

        public SwiftSearchLauncher(Window window) : base(window)
        {
        }

        public void SetStartShortcut(KeyModifyers modifyers, Keys key)
        {
            if (isSetStartShortcut) return;

            isSetStartShortcut = Add(modifyers, (int) key, Start);
        }

        private void Start()
        {
            IoC.ServiceLocator.Get<GlobalShortcutService>().Stop();
            launcher.Launch();
            launcher.Window.Closed += (s, e) => IoC.ServiceLocator.Get<GlobalShortcutService>().Start();
            ;
        }

        public void SetStartFromClipboardShortcut(KeyModifyers modifyers, Keys key)
        {
            if (isSetStartFromClipboardShortcut) return;

            isSetStartFromClipboardShortcut = Add(modifyers, (int) key, StartFromClipboard);
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