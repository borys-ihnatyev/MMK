namespace MMK.KeyDrive.Models.Holders
{
    public partial class Holder
    {
        public class NotSupportedException : Exception
        {
            public NotSupportedException(string path)
                : base("File ("+path+") extension is not supported")
            {

            }
        }
    }
}
