using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace MMK.Notify.Observer.Tasking
{
    public sealed class TaskGroup
    {
        private readonly Guid id;

        private readonly Dictionary<GroupedTask, Task.ObservedInfo> taskProcessing;

        public TaskGroup(IEnumerable<Task> tasks)
        {
            if (tasks == null)
                throw new ArgumentNullException("tasks");
            Contract.EndContractBlock();

            var taskArray = tasks as Task[] ?? tasks.ToArray();

            if (!Contract.ForAll(taskArray, t => t != null)) 
                throw new ArgumentException("contains nulls","tasks");


            taskProcessing = new Dictionary<GroupedTask, Task.ObservedInfo>();
            Wrap(taskArray);

            id = Guid.NewGuid();
        }

        public IEnumerable<Task> Tasks
        {
            get
            {
                Contract.Ensures(Contract.Result<IEnumerable<Task>>() != null);
                return taskProcessing.Keys;
            }
        }

        public int Capicity
        {
            get { return taskProcessing.Keys.Count; }
        }

        private void Wrap(IEnumerable<Task> tasks)
        {
            foreach (var task in tasks)
                taskProcessing.Add(new GroupedTask(task, this), null);
        }

        public bool IsProcessed
        {
            get { return taskProcessing.All(i => i.Value != null); }
        }

        public bool TryMarkProcessed(Task.ObservedInfo info)
        {
            Contract.Requires(info != null);

            if (!IsOwn(info.Task)) return false;

            Contract.Assume(info.Task is GroupedTask);
            taskProcessing[info.Task as GroupedTask] = info;

            return true;
        }

        public bool IsOwn(Task task)
        {
            var groupedTask = task as GroupedTask;
            if (groupedTask == null) return false;

            return groupedTask.Group == this;
        }

        public Task.ObservedInfo GetDoneTasksObservedInfo()
        {
            return GetObservedInfo(i => i.IsOk, "Done");
        }

        public Task.ObservedInfo GetFailedTasksObservedInfo()
        {
            return GetObservedInfo(i => i.IsFailed, "Failed");
        }

        private Task.ObservedInfo GetObservedInfo(Func<Task.ObservedInfo, bool> selector, string description) 
        {
            Contract.Assert(IsProcessed);

            var matchedTasks = taskProcessing.Values.Where(selector).ToArray();

            if (matchedTasks.Length == 0) return null;

            var example = matchedTasks[0];

            Contract.Assert(example != null);

            if (matchedTasks.Length == 1)
                return example;

            return new Task.ObservedInfo(example)
            {
                DetailedDescription = BuildDetailedDescription(description, matchedTasks.Length),
                TargetObject = ""
            };

        }

        private string BuildDetailedDescription(string description, int total)
        {
            var detailedDescription = Capicity == total
                ? string.Format("All {0} {1}", Capicity, description)
                : string.Format("{0} {1}/{2}", description, total, Capicity);
            return detailedDescription;
        }

        public override int GetHashCode()
        {
            return id.GetHashCode();
        }
    }
}