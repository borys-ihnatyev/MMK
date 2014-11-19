using System;
using System.IO;
using System.Runtime.InteropServices;

namespace MMK.KeyDrive.Models
{
    public class DriveDetector
    {
        private const int WmDeviceChange = 0x0219;
        private const int DbtDeviceArrival = 0x8000;
        private const int DbtDeviceRemoveComplete = 0x8004;
        private const int DbtDevTypeVolume = 0x00000002;

        public event EventHandler<EventArgs<DriveInfo>> DriveConnected;
        public event EventHandler<EventArgs<DriveInfo>> DriveRemoved;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="lParam"></param>
        /// <param name="wParam"></param>
        /// <returns>Handled</returns>
        public bool WndProc(int message, IntPtr lParam, IntPtr wParam)
        {
            if (message != WmDeviceChange) return false;
            
            var dbt = wParam.ToInt32();
            
            if (dbt == DbtDeviceArrival)
                return OnDeviceConnected(lParam);
            
            if (dbt == DbtDeviceRemoveComplete)
                return OnDeviceRemoved(lParam);

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="deviceHandle"></param>
        /// <returns>handeled</returns>
        private bool OnDeviceConnected(IntPtr deviceHandle)
        {
            if (!IsDeviceTypeVolume(deviceHandle))
                return false;

            var driveInfo = DriveInfoFactory.FromDriveHandle(deviceHandle);
            OnDriveConnected(driveInfo);

            return true;
        }

        protected virtual void OnDriveConnected(DriveInfo drive)
        {
            if (DriveConnected == null) return;
            var e = new EventArgs<DriveInfo>(drive);
            DriveConnected(this, e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="deviceHandle"></param>
        /// <returns>handeled</returns>
        private bool OnDeviceRemoved(IntPtr deviceHandle)
        {
            if (!IsDeviceTypeVolume(deviceHandle))
                return false;

            var driveInfo = DriveInfoFactory.FromDriveHandle(deviceHandle);

            OnDriveRemoved(driveInfo);
            return true;
        }

        protected virtual void OnDriveRemoved(DriveInfo drive)
        {
            if (DriveRemoved == null) return;
            var e = new EventArgs<DriveInfo>(drive);
            DriveRemoved(this, e);
        }

        private static bool IsDeviceTypeVolume(IntPtr handle)
        {
            var devType = Marshal.ReadInt32(handle, 4);
            return devType == DbtDevTypeVolume;
        }
    }
}