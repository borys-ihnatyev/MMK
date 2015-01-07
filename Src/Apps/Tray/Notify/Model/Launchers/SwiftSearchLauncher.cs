using System;
using System.Windows;
using System.Windows.Forms;
using MMK.ApplicationServiceModel;
using MMK.Notify.Services;
using MMK.SwiftSearch.ViewModels;
using MMK.SwiftSearch.Views;
using MMK.Wpf;
using MMK.Wpf.Providers;
using Clipboard = System.Windows.Clipboard;

namespace MMK.Notify.Model.Launchers
{
    public sealed class SwiftSearchLauncher : GlobalShortcutProviderCollection
    {
        private readonly SwiftSearchViewModel viewModel;
        private readonly WindowLauncher<SwiftSearchView> launcher;

        private bool isSetStartShortcut;
        private bool isSetStartFromClipboardShortcut;

        public SwiftSearchLauncher() : this(null)
        {
        }

        public SwiftSearchLauncher(Window ownerWindow) : base(ownerWindow)
        {
            viewModel = new SwiftSearchViewModel();
            launcher = new WindowLauncher<SwiftSearchView>(() => new SwiftSearchView(viewModel));
        }

        public void SetStartShortcut(KeyModifyers modifyers, Keys key)
        {
            if (isSetStartShortcut) return;

            isSetStartShortcut = Add(modifyers, (int) key, Start);
        }

        public void SetStartFromClipboardShortcut(KeyModifyers modifyers, Keys key)
        {
            if (isSetStartFromClipboardShortcut) return;
            isSetStartFromClipboardShortcut = Add(modifyers, (int) key, StartFromClipboard);
        }

        private void Start()
        {
            IoC.Get<GlobalShortcutService>().Stop();
            viewModel.Search = String.Empty;
            launcher.Launch();
            launcher.Window.Closed += (s, e) => IoC.Get<GlobalShortcutService>().Start();
        }

        private void StartFromClipboard()
        {
            viewModel.Search = String.Empty;
            if (Clipboard.ContainsText())
                viewModel.Search = Clipboard.GetText();
            Start();
        }
    }
}