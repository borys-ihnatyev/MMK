using System.Collections.Generic;
using MMK.Marking.Representation;
using MMK.Notify.Observer.Tasking;
using MMK.Processing.AutoFolder;

namespace MMK.Notify.Observer
{
    public interface INotifyObserver
    {
        void TestConnection();

        void Observe(Task task);
        void Observe(IEnumerable<Task> task);
    }
}