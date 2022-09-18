namespace MMK.Utils.Media
{
    public interface IColorArgbModel<out T> where T : struct
    {
        T A { get; }
        T R { get; }
        T G { get; }
        T B { get; }
    }
}