using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;

namespace MMK.ApplicationServiceModel.Locator.Resolving
{

    internal class ServiceLocatorResolver
    {
        private readonly Assembly assembly;
        private Type serviceLocatorOwnerType;
        private PropertyInfo serviceLocatorPropertyInfo;

        public ServiceLocatorResolver(Assembly assembly)
        {
            if(assembly == null)
                throw new ArgumentNullException("assembly");
            Contract.EndContractBlock();

            this.assembly = assembly;
        }

        public object GetServiceLocator()
        {
            serviceLocatorOwnerType = GetServiceLocatorOwnerType();
            serviceLocatorPropertyInfo = GetServiceLocatorOwnerPropertyInfo();
            return serviceLocatorPropertyInfo.GetValue(null,null);
        }

        private Type GetServiceLocatorOwnerType()
        {
            var serviceLocatorOwners = assembly.GetTypes()
                .Where(t => Attribute.IsDefined(t, typeof(ServiceLocatorOwnerAttribute)))
                .ToList();

            if(serviceLocatorOwners.Count == 0)
                throw new ServiceLocatorNotFoundException("Class marked with ServiceLocatorOwnerAttribute was not found in assembly " + assembly);

            if (serviceLocatorOwners.Count > 1)
                throw new ServiceLocatorConflictException();

            return serviceLocatorOwners[0];
        }

        private PropertyInfo GetServiceLocatorOwnerPropertyInfo()
        {
            Contract.Ensures(serviceLocatorOwnerType != null);
            Contract.EndContractBlock();

            var serviceLocatorProperties = serviceLocatorOwnerType
                .GetProperties(BindingFlags.Public | BindingFlags.Static)
                .Where(p => Attribute.IsDefined(p, typeof (ServiceLocatorAttribute)))
                .ToList();

            if (serviceLocatorProperties.Count == 0)
                throw new ServiceLocatorNotFoundException("Property marked with ServiceLocatorAttribute was not found in instance of type"+serviceLocatorOwnerType);

            if(serviceLocatorProperties.Count > 1)
                throw new ServiceLocatorConflictException("Only one property can be marked with ServiceLocatorAttribute");

            return serviceLocatorProperties[0];
        }
    }
}