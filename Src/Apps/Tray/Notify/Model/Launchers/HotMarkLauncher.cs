using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using MMK.HotMark.View;
using MMK.Wpf;
using MMK.Wpf.Providers;

namespace MMK.Notify.Model.Launchers
{
    public sealed class HotMarkLauncher : GlobalShortcutProvider
    {
        private readonly HotMarkWindowLauncher launcher;

        public HotMarkLauncher(KeyModifyers modifyer, int keyCode) : base(modifyer, keyCode)
        {
            launcher = new HotMarkWindowLauncher();
            Pressed += OnPressed;
        }

        private void OnPressed()
        {
            var items = Explorer.GetForegroundSelectedItemsPaths()
                .Where(path => File.Exists(path) || Directory.Exists(path))
                .ToList();

            if (items.Any())
                launcher.Launch(items);
        }

        private class HotMarkWindowLauncher : WindowLauncher<MainView>
        {
            private IEnumerable<string> paths = new string[0];

            public void Launch(IEnumerable<string> filePaths)
            {
                paths = filePaths;

                Launch();
            }

            protected override MainView WindowFactory()
            {
                return new MainView(paths);
            }

            protected override void BeforeLaunch()
            {
                App.Current.StopListenShortcuts();
            }

            protected override void BindWindowEvents()
            {
                base.BindWindowEvents();
                Window.Closed += (sender, args) => App.Current.StartListenShortcuts();
            }
        }
    }
}