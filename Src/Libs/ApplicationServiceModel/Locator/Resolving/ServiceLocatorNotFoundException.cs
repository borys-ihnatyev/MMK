using System;

namespace MMK.ApplicationServiceModel.Locator.Resolving
{
    [Serializable]
    public class ServiceLocatorNotFoundException : Exception
    {
        public ServiceLocatorNotFoundException(string message) : base(message)
        {
            
        }
    }
}