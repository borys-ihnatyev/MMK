using System;

namespace MMK.ApplicationServiceModel.Locator.Resolving
{
    public class ServiceLocatorOwnerNotFoundException : Exception
    {
        public ServiceLocatorOwnerNotFoundException(string message) : base(message)
        { }
    }
}