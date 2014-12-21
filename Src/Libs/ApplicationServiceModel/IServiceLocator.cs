namespace MMK.ApplicationServiceModel
{
    public interface IServiceLocator
    {
        T Get<T>();
    }
}