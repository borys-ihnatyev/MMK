using System;
using System.Runtime.Serialization;

namespace MMK.Notify.Observer.Tasking
{
    [Serializable]
    public class InvalidTaskContextException : Exception
    {

        public InvalidTaskContextException()
        {
        }

        public InvalidTaskContextException(string message) : base(message)
        {
        }

        public InvalidTaskContextException(string message, Exception inner) : base(message, inner)
        {
        }

        protected InvalidTaskContextException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}