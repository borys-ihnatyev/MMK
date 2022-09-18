using System;
using System.IO;
using System.Runtime.InteropServices;
using MMK.ApplicationServiceModel;
using MMK.KeyDrive.Models.IO;
using MMK.Presentation.Windows.Interop;

namespace MMK.KeyDrive.Services
{
    public abstract class DriveDetectorService : IService
    {
        private const int WmDeviceChange = 0x0219;
        private const int DbtDeviceArrival = 0x8000;
        private const int DbtDeviceRemoveComplete = 0x8004;
        private const int DbtDevTypeVolume = 0x00000002;
        
        private readonly IHwndSource hwndSource;
        private bool isHooked;

        protected DriveDetectorService(IHwndSource hwndSource)
        {
            this.hwndSource = hwndSource;
        }

        public void Start()
        {
            if(isHooked)
                return;

            hwndSource.AddHook(Hook);

            isHooked = true;
        }

        public void Stop()
        {
            if(!isHooked)
                return;
            hwndSource.RemoveHook(Hook);
            isHooked = false;
        }

        private IntPtr Hook(IntPtr hwnd, int message, IntPtr lParam, IntPtr wParam, ref bool handled)
        {
            if (message == WmDeviceChange)
            {
                var dbt = wParam.ToInt32();

                switch (dbt)
                {
                    case DbtDeviceArrival:
                        handled = OnDeviceConnected(lParam);
                        break;
                    case DbtDeviceRemoveComplete:
                        handled = OnDeviceRemoved(lParam);
                        break;
                }
            }

            return IntPtr.Zero;
        }

        private bool OnDeviceConnected(IntPtr deviceHandle)
        {
            if (!IsDeviceTypeVolume(deviceHandle))
                return false;

            var driveInfo = DriveInfoFactory.FromDriveHandle(deviceHandle);
            OnDriveConnected(driveInfo);
            return true;
        }

        protected abstract void OnDriveConnected(DriveInfo driveInfo);

        private bool OnDeviceRemoved(IntPtr deviceHandle)
        {
            if (!IsDeviceTypeVolume(deviceHandle))
                return false;

            var driveInfo = DriveInfoFactory.FromDriveHandle(deviceHandle);
            OnDriveRemoved(driveInfo);
            return true;
        }

        protected abstract void OnDriveRemoved(DriveInfo driveInfo);

        private static bool IsDeviceTypeVolume(IntPtr handle)
        {
            var devType = Marshal.ReadInt32(handle, 4);
            return devType == DbtDevTypeVolume;
        }
    }
}