using System;
using System.IO;
using MMK.Presentation.Windows.Interop;

namespace MMK.KeyDrive.Services
{
    public sealed class DriveDetectorService : DriveDetectorServiceBase
    {
        public DriveDetectorService(IHwndSource hwndSource) : base(hwndSource)
        {
        }

        protected override void OnDriveConnected(DriveInfo driveInfo)
        {
            var handler = DriveConnected;
            if (handler != null)
                handler(this, new DriveEventArg(driveInfo));
        }

        protected override void OnDriveRemoved(DriveInfo driveInfo)
        {
            var handler = DriveRemoved;
            if (handler != null)
                handler(this, new DriveEventArg(driveInfo));
        }

        public event EventHandler<DriveEventArg> DriveConnected;
        public event EventHandler<DriveEventArg> DriveRemoved;
    }
}