using System.IO;

namespace MMK.KeyDrive.Models.Holders
{
    public partial class Holder
    {
        public class NotFoundException : Exception
        {
            public NotFoundException()
            {
                
            }

            public NotFoundException(IOException innerException)
                : base("Item not found", innerException)
            {

            }
        }
    }
}
