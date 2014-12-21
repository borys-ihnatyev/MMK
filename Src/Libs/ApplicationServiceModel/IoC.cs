using System.Reflection;
using MMK.ApplicationServiceModel.Locator.Resolving;

namespace MMK.ApplicationServiceModel
{
    public class IoC
    {
        private static IServiceLocator assemblyServiceLocator;

        public static IServiceLocator ServiceLocator
        {
            get
            {
                if (assemblyServiceLocator == null)
                {
                    var serviceLocatorResolver = new ServiceLocatorResolver(Assembly.GetEntryAssembly());
                    assemblyServiceLocator = serviceLocatorResolver.ResolveLocator();
                }

                return assemblyServiceLocator;
            }
        }
    }
}