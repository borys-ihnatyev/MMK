using System;

namespace MMK.Application.ServiceLocator.Resolving
{
    [Serializable]
    public class ServiceLocatorNotFoundException : Exception
    {
        public ServiceLocatorNotFoundException(string message) : base(message)
        {
            
        }
    }
}