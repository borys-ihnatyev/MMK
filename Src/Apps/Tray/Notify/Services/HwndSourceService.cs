using System;
using System.Diagnostics.Contracts;
using System.Windows;
using System.Windows.Interop;
using MMK.ApplicationServiceModel;
using MMK.Presentation;
using MMK.Presentation.Windows.Interop;

namespace MMK.Notify.Services
{
    public sealed class HwndSourceService : InitializableService, IHwndSource
    {
        private TrayWindow serviceWindow;
        private HwndSource hwndSource;

        protected override void OnInitialize()
        {
            Contract.Ensures(hwndSource != null);
            Contract.EndContractBlock();

            hwndSource = PresentationSource.FromVisual(serviceWindow) as HwndSource;
        }

        protected override void OnStart()
        {
            Contract.Ensures(serviceWindow != null);
            Contract.EndContractBlock();

            Contract.Assume(serviceWindow == null);

            serviceWindow = new TrayWindow
            {
                WindowStyle = WindowStyle.None,
                Width = 0,
                Height = 0,
                Visibility = Visibility.Hidden
            };

            serviceWindow.SourceInitialized += (s, e) => Initialize();
            serviceWindow.Show();
        }

        protected override void OnStop()
        {
            Contract.Ensures(serviceWindow == null);
            Contract.EndContractBlock();

            Contract.Assume(serviceWindow != null);
            serviceWindow.Close();
            serviceWindow = null;
        }

        public IntPtr Handle
        {
            get
            {
                CheckInitialized();
                return hwndSource.Handle;
            }
        }

        public void AddHook(HwndSourceHook hook)
        {
            CheckInitialized();
            Contract.Assume(hwndSource != null);
            hwndSource.AddHook(hook);
        }

        public void RemoveHook(HwndSourceHook hook)
        {
            CheckInitialized();
            Contract.Assume(hwndSource != null);
            hwndSource.RemoveHook(hook);
        }
    }
}