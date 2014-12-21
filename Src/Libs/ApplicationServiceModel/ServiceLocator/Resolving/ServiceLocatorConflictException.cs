using System;

namespace MMK.Application.ServiceLocator.Resolving
{
    [Serializable]
    public class ServiceLocatorConflictException : Exception
    {
        public ServiceLocatorConflictException()
        {
        }

        public ServiceLocatorConflictException(string message):base(message)
        {
        }
    }
}