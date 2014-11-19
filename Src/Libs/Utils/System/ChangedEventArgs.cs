namespace System
{
    public class ChangedEventArgs<T> : EventArgs
    {
        public ChangedEventArgs(T oldValue, T newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }

        public T OldValue
        {
            get;
            private set;
        }

        public T NewValue
        {
            get;
            private set;
        }
    }
}