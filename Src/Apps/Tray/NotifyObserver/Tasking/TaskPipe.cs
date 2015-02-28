using System;
using System.Collections.Generic;

namespace MMK.Notify.Observer.Tasking
{
    public sealed class TaskPipe
    {
        private readonly ITaskContext context;
        private readonly INotifyObserver observer;

        private readonly LinkedList<Task> tasks;

        internal TaskPipe(ITaskContext context, INotifyObserver observer)
        {
            this.context = context;
            this.observer = observer;
            tasks = new LinkedList<Task>();
        }

        public TaskPipe Pipe(Task task)
        {
            task.Context = context;
            tasks.AddLast(task);
            return this;
        }

        public void Observe(TimeSpan deelay)
        {
            observer.Observe(tasks, deelay);
        }

        public void Observe()
        {
            observer.Observe(tasks);
        }
    }
}