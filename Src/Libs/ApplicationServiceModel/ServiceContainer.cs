using System;

namespace MMK.Application
{
    public sealed class ServiceContainer : IServiceContainer
    {
        public TService GetService<TService>()
        {
            throw new NotImplementedException();
        }
    }
}
