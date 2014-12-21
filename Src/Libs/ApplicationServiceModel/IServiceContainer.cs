namespace MMK.Application
{
    public interface IServiceContainer
    {
        TService GetService<TService>();
    }
}