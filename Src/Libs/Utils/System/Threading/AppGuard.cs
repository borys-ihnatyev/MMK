using System.Reflection;

// ReSharper disable once CheckNamespace
namespace System.Threading
{
    public static class AppGuard
    {
        public static bool IsSingleInstance()
        {
            var entryAssemblyName = Assembly.GetEntryAssembly().FullName;
            bool isSingleInstance;
            new Mutex(true, entryAssemblyName, out isSingleInstance);
            return isSingleInstance;
        }
    }
}
