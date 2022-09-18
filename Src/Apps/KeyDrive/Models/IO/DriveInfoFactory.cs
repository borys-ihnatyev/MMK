using System;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;

namespace MMK.KeyDrive.Models.IO
{
    public static partial class DriveInfoFactory
    {
        public static DriveInfo FromDriveHandle(IntPtr volumeInfo)
        {
            if(volumeInfo == null || volumeInfo == IntPtr.Zero)
                throw new ArgumentNullException("volumeInfo");
            Contract.EndContractBlock();

            var volume = (DeviceBroadcastVolume)Marshal.PtrToStructure(volumeInfo, typeof(DeviceBroadcastVolume));
            var letter = DriveLetterFromMask(volume.unitMask);

            return new DriveInfo(letter.ToString(CultureInfo.InvariantCulture));
        }

        private static char DriveLetterFromMask(int mask)
        {
            var driveLetter = (char)('A' - 1);
            do
            {
                mask = mask / 2;
                ++driveLetter;
            } while (mask != 0);

            return driveLetter;
        }
    }
}