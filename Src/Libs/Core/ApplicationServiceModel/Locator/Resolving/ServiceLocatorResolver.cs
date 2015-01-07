using System;
using System.Collections.Generic;
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

        public IServiceLocator ResolveLocator()
        {
            serviceLocatorOwnerType = GetLocatorOwnerType();
            serviceLocatorPropertyInfo = GetLocatorPropertyType();
            return (IServiceLocator)serviceLocatorPropertyInfo.GetValue(null,null);
        }

        private Type GetLocatorOwnerType()
        {
            var serviceLocatorOwners = GetOwners();
            CheckOwnerIsOne(serviceLocatorOwners);
            return serviceLocatorOwners[0];
        }

        private List<Type> GetOwners()
        {
            return assembly.GetTypes()
                .Where(t => Attribute.IsDefined(t, typeof(ServiceLocatorOwnerAttribute)))
                .ToList();
        }

        [ContractInvariantMethod]
        private void CheckOwnerIsOne(List<Type> serviceLocatorOwners)
        {
            if (serviceLocatorOwners.Count == 0)
                throw new ServiceLocatorNotFoundException(
                    "Class marked with ServiceLocatorOwnerAttribute was not found in assembly " + assembly);
            if (serviceLocatorOwners.Count > 1)
                throw new ServiceLocatorConflictException();
        }

        private PropertyInfo GetLocatorPropertyType()
        {
            var serviceLocatorProperties = GetLocatorProperties();
            CheckLocatorPropertyIsOneAndMatchType(serviceLocatorProperties);
            return serviceLocatorProperties[0];
        }

        private List<PropertyInfo> GetLocatorProperties()
        {
            Contract.Ensures(serviceLocatorOwnerType != null);
            Contract.EndContractBlock();

            return serviceLocatorOwnerType
                .GetProperties(BindingFlags.Public | BindingFlags.Static)
                .Where(p => Attribute.IsDefined(p, typeof (ServiceLocatorAttribute)))
                .ToList();
        }

        [ContractInvariantMethod]
        private void CheckLocatorPropertyIsOneAndMatchType(IReadOnlyList<PropertyInfo> serviceLocatorProperties)
        {
            if (serviceLocatorProperties.Count == 0)
                throw new ServiceLocatorNotFoundException("Property marked with ServiceLocatorAttribute was not found in instance of type" + serviceLocatorOwnerType);

            if (serviceLocatorProperties.Count > 1)
                throw new ServiceLocatorConflictException("Only one property can be marked with ServiceLocatorAttribute");

            if (!typeof (IServiceLocator).IsAssignableFrom(serviceLocatorProperties[0].PropertyType))
                throw new ServiceLocatorNotFoundException("Property marked with ServiceLocatorAttribute must be subclass of IServiceLocator");
        }
    }
}