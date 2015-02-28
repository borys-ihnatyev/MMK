using System;
using System.Collections.Generic;
using MMK.ApplicationServiceModel;
using MMK.Notify.Observer.Tasking;

namespace MMK.Notify.Observer
{
    public interface INotifyObserver: IService
    {
        void TestConnection();

        void Observe(Task task);
        void Observe(Task task, TimeSpan deelay);

        void Observe(IEnumerable<Task> task);
        void Observe(IEnumerable<Task> task, TimeSpan deelay);

        TaskPipe Using(ITaskContext context);
    }
}