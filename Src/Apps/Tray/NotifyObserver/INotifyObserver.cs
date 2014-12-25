using System.Collections.Generic;
using MMK.Notify.Observer.Tasking;

namespace MMK.Notify.Observer
{
    public interface INotifyObserver
    {
        void TestConnection();

        void Observe(Task task);
        void Observe(IEnumerable<Task> task);
    }
}