using System.Collections.Generic;

namespace System.IO
{
    public class DriveInfoNameEqualityComparer : IEqualityComparer<DriveInfo>
    {
        public bool Equals(DriveInfo x, DriveInfo y)
        {
            if (x == null && y == null)
                return true;
            if (x == null || y == null)
                return false;

            return x.Name[0] == y.Name[0];
        }

        public int GetHashCode(DriveInfo obj)
        {
            return obj == null ? 0 : obj.Name[0].GetHashCode();
        }
    }
}
