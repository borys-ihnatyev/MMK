using System.Reflection;
using MMK.ApplicationServiceModel.ServiceLocator.Resolving;

namespace MMK.ApplicationServiceModel
{
    public class IoC
    {
        private static object current;

        public static T Get<T>()
        {
            return (T)(current ?? (current = new ServiceLocatorResolver(Assembly.GetEntryAssembly()).GetServiceLocator()));
        }
    }
}