using System;
using System.IO;

namespace MMK.KeyDrive.Models.Holders
{
    public partial class Holder
    {
        [Serializable]
        public class Exception : System.Exception
        {
            public Exception(string message, System.Exception innerException) : base(message, innerException)
            {

            }

            public Exception(string message) : base(message)
            {

            }

            public Exception()
            {
            }
        }
    }
}
