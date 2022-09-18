using System;

namespace MMK.ApplicationServiceModel.Locator.Resolving
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