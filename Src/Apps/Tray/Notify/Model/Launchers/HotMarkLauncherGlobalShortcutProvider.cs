using System.Collections.Generic;
using System.Linq;
using System.Windows;
using MMK.ApplicationServiceModel;
using MMK.HotMark.ViewModels;
using MMK.HotMark.Views;
using MMK.Notify.Services;
using MMK.Wpf;
using MMK.Wpf.Providers;

namespace MMK.Notify.Model.Launchers
{
    public sealed class HotMarkLauncherGlobalShortcutProvider : GlobalShortcutProvider
    {
        private readonly HotMarkWindowLauncher launcher;

        public HotMarkLauncherGlobalShortcutProvider(KeyModifyers modifyer, int keyCode) : base(modifyer, keyCode)
        {
            launcher = new HotMarkWindowLauncher();
            Pressed += OnPressed;
        }

        private void OnPressed()
        {
            var filesEnumer = Explorer.GetForegroundSelectedItemsFileTree(".mp3");
            var files = filesEnumer as IList<string> ?? filesEnumer.ToList();
            if (files.Any())
                launcher.Launch(files);
        }

        private class HotMarkWindowLauncher : WindowLauncher<HotMarkMainView>
        {
            private IEnumerable<string> paths = new string[0];

            public void Launch(IEnumerable<string> filePaths)
            {
                paths = filePaths;
                Launch();
            }

            protected override HotMarkMainView WindowFactory()
            {
                var viewModel = new HotMarkViewModel(paths);
                return new HotMarkMainView(viewModel);
            }

            protected override void BeforeLaunch()
            {
                IoC.Get<GlobalShortcutService>().Stop();
            }

            protected override void BindWindowEvents()
            {
                base.BindWindowEvents();
                Window.Closed += (sender, args) => IoC.Get<GlobalShortcutService>().Start();
            }
        }
    }
}