using System.Reflection;
using MMK.Application.ServiceLocator.Resolving;

namespace MMK.Application
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