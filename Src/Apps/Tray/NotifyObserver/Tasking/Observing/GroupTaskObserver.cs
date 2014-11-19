using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace MMK.Notify.Observer.Tasking.Observing
{
    public class GroupTaskObserver : TaskObserver
    {
        protected override IEnumerable<Task> BeforeQueue(IEnumerable<Task> newTasks)
        {
            var taskArray = newTasks as Task[] ?? newTasks.ToArray();

            return taskArray.Length > 1
                ? new TaskGroup(taskArray).Tasks
                : taskArray;
        }

        protected override void OnTaskObserved(Task.ObservedInfo info)
        {
            if (info.Task is GroupedTask)
                OnGroupedTaskObserved(info);
            else
                base.OnTaskObserved(info);
        }

        private void OnGroupedTaskObserved(Task.ObservedInfo info)
        {
            var task = info.Task as GroupedTask;
            Contract.Assume(task != null);
            var group = task.Group;

            group.TryMarkProcessed(info);

            if (task.Group.IsProcessed)
                OnGroupProcessed(group);
        }

        private void OnGroupProcessed(TaskGroup group)
        {
            var mergedInfo = group.GetDoneTasksObservedInfo();

            if (mergedInfo != null)
                base.OnTaskObserved(mergedInfo);

            mergedInfo = group.GetFailedTasksObservedInfo();

            if (mergedInfo != null)
                base.OnTaskObserved(mergedInfo);
        }
    }
}