using System.Collections.Generic;
using System.Reflection;
using MMK.ApplicationServiceModel.Locator.Resolving;

namespace MMK.ApplicationServiceModel
{
    public class IoC
    {
        private static IServiceLocator entryServiceLocator;

        private static readonly Dictionary<Assembly, IServiceLocator> AssemblyServiceLocators = new Dictionary<Assembly, IServiceLocator>(); 

        private static IServiceLocator EntryServiceLocator
        {
            get
            {
                if (entryServiceLocator == null)
                {
                    var serviceLocatorResolver = new ServiceLocatorResolver(Assembly.GetEntryAssembly());
                    entryServiceLocator = serviceLocatorResolver.ResolveLocator();
                }

                return entryServiceLocator;
            }
        }

        private static IServiceLocator TryGetServiceLocator(Assembly assembly)
        {
            if (assembly == Assembly.GetEntryAssembly())
                return EntryServiceLocator;

            if (AssemblyServiceLocators.ContainsKey(assembly))
                return AssemblyServiceLocators[assembly];

            var resolver = new ServiceLocatorResolver(assembly);
            var serviceLocator = resolver.TryResolveLocator();
            AssemblyServiceLocators.Add(assembly,serviceLocator);
            return serviceLocator;
        }

        public static TService Get<TService>() where TService : class 
        {
            var callingAssembly = Assembly.GetCallingAssembly();
            var serviceLocator = TryGetServiceLocator(callingAssembly) ?? EntryServiceLocator;
            var service = serviceLocator.Get<TService>();
            return service == null 
                ? EntryServiceLocator.Get<TService>() 
                : serviceLocator.Get<TService>();
        }
    }
}