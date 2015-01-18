using System;
using System.Diagnostics.Contracts;
using System.Windows;
using System.Windows.Interop;
using MMK.ApplicationServiceModel;
using MMK.Presentation;
using MMK.Presentation.Windows.Interop;

namespace MMK.Notify.Services
{
    public class HwndSourceService : Service, IHwndSource
    {
        private TrayWindow serviceWindow;
        private HwndSource hwndSource;

        protected override void OnInitialize()
        {
            Contract.Ensures(hwndSource != null);
            Contract.EndContractBlock();

            hwndSource = PresentationSource.FromVisual(serviceWindow) as HwndSource;
        }

        public override void Start()
        {
            if(serviceWindow != null)
                return;

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

        public override void Stop()
        {
            if(serviceWindow == null)
                return;

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
            hwndSource.AddHook(hook);
        }

        public void RemoveHook(HwndSourceHook hook)
        {
            CheckInitialized();
            hwndSource.RemoveHook(hook);
        }
    }
}