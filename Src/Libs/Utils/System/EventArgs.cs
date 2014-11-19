namespace System
{
    public class EventArgs<T> : EventArgs
    {
        public EventArgs(T arg)
        {
            Arg = arg;
        }

        public T Arg
        {
            get;
            private set;
        }
    }
}
