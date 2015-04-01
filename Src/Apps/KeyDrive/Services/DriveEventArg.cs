using System;
using System.IO;

namespace MMK.KeyDrive.Services
{
    public class DriveEventArg : EventArgs
    {
        internal DriveEventArg(DriveInfo driveInfo)
        {
            DriveInfo = driveInfo;
        }

        public DriveInfo DriveInfo { get; private set; }
    }
}