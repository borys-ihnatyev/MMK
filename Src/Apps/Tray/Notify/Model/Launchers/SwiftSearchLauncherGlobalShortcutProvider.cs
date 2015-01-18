using System;
using System.Windows;
using System.Windows.Input;
using MMK.ApplicationServiceModel;
using MMK.Notify.Services;
using MMK.Presentation;
using MMK.Presentation.Windows.Input;
using MMK.SwiftSearch.ViewModels;
using MMK.SwiftSearch.Views;

namespace MMK.Notify.Model.Launchers
{
    public sealed class SwiftSearchLauncherGlobalShortcutProvider : GlobalShortcutProviderCollection
    {
        private readonly SwiftSearchViewModel viewModel;
        private readonly WindowLauncher<SwiftSearchView> launcher;

        private bool isSetStartShortcut;
        private bool isSetStartFromClipboardShortcut;

        public SwiftSearchLauncherGlobalShortcutProvider() : this(null)
        {
        }

        public SwiftSearchLauncherGlobalShortcutProvider(Window window) : base(window)
        {
            viewModel = new SwiftSearchViewModel();
            launcher = new WindowLauncher<SwiftSearchView>(() => new SwiftSearchView(viewModel));
        }

        public void SetStartShortcut(ModifierKeys modifiers, System.Windows.Input.Key key)
        {
            if (isSetStartShortcut) return;

            isSetStartShortcut = Add(modifiers, key, Start) != null;
        }

        public void SetStartFromClipboardShortcut(ModifierKeys modifiers, System.Windows.Input.Key key)
        {
            if (isSetStartFromClipboardShortcut) return;
            isSetStartFromClipboardShortcut = Add(modifiers, key, StartFromClipboard) != null;
        }

        private void Start()
        {
            Start(String.Empty);
        }

        private void Start(string search)
        {
            IoC.Get<GlobalShortcutService>().Stop();
            viewModel.Search = search;
            launcher.Launch();
            launcher.Window.Closed += (s, e) => IoC.Get<GlobalShortcutService>().Start();
        }

        private void StartFromClipboard()
        {
            var search = String.Empty;
            if (Clipboard.ContainsText())
                search = Clipboard.GetText();
            Start(search);
        }
    }
}