using MMK.ApplicationServiceModel;
using Ninject;

namespace MMK.Notify.Model
{
    public class ServiceLocator : StandardKernel, IServiceLocator
    {
        T IServiceLocator.Get<T>()
        {
            return this.Get<T>();
        }
    }
}