using System;

namespace MMK.Notify.Observer.Tasking
{
    public partial class Task
    {
        [Serializable]
        public class Cancel : Exception
        {
            public Cancel()
            {
            }

            public Cancel(string message) : base(message)
            {
            }

            public Cancel(string message, Exception innerException) : base(message, innerException)
            {
            }
        }
    }
}