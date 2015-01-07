using System;

namespace MMK.ApplicationServiceModel.Locator
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