using System;

namespace MMK.Application.ServiceLocator
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ServiceLocatorOwnerAttribute : Attribute
    {
        public string PropertyName
        {
            get; set;
        }
    }
}