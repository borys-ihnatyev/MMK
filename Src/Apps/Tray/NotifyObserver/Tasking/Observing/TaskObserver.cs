using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using Timer = System.Timers.Timer;

namespace MMK.Notify.Observer.Tasking.Observing
{
    public class TaskObserver : IDisposable
    {
        public const int FailedTaskRerunCount = 10;
        public const int FailedTaskRerunPauseSeconds = 5;

        private readonly ManualResetEvent taskSyncEvent;
        private readonly ManualResetEvent taskRunEvent;
        private readonly AutoResetEvent taskCancelEvent;

        public event Action<INotifyable> TaskDone;
        public event Action<INotifyable> TaskObserved;
        public event Action<INotifyable> TaskFailed;

        private Thread thread;
        private readonly Queue<Task> tasks;

        public TaskObserver()
        {
            tasks = new Queue<Task>();

            taskSyncEvent = new ManualResetEvent(false);
            taskRunEvent = new ManualResetEvent(false);
            taskCancelEvent = new AutoResetEvent(false);

            thread = new Thread(TaskObserverProc);
        }

        public int ReceivedTasksCount
        {
            get { return tasks.Count; }
        }

        protected virtual void OnTaskObserved(Task.ObservedInfo info)
        {
            Contract.Requires(info != null);

            if (TaskObserved != null)
                TaskObserved(info);

            if (info.IsOk)
                OnTaskDone(info);
            else
                OnTaskFailed(info);
        }

        protected virtual void OnTaskDone(Task.ObservedInfo taskObservedInfo)
        {
            if (TaskDone != null)
                TaskDone(taskObservedInfo);
        }

        protected virtual void OnTaskFailed(Task.ObservedInfo taskObservedInfo)
        {
            if (TaskFailed != null)
                TaskFailed(taskObservedInfo);
        }

        #region Flow

        private void TaskObserverProc()
        {
            while (HandleEvents())
            {
                ObserveTask(tasks.Dequeue());
                CheckForTasks();
            }
        }

        private bool HandleEvents()
        {
            taskSyncEvent.Set();

            taskRunEvent.WaitOne();

            taskSyncEvent.Reset();

            return !taskCancelEvent.WaitOne(0);
        }

        private void ObserveTask(Task task)
        {
            try
            {
                var taskObservedInfo = task.Run();

                if (IsNeedRerunTask(taskObservedInfo))
                {
                    taskObservedInfo.DetailedDescription += "\n Retry once.";
                    taskObservedInfo.Type = NotifyType.Warning;
                    AddTaskWithDeelay(task);
                }
                else if (taskObservedInfo.IsFailed)
                {
                    taskObservedInfo.DetailedDescription = taskObservedInfo.DetailedDescription.Replace(
                        "\n Retry once.", "");
                    taskObservedInfo.Type = NotifyType.Error;
                }

                OnTaskObserved(taskObservedInfo);
            }
            catch (Task.Cancel)
            {

            }
        }

        private bool IsNeedRerunTask(Task.ObservedInfo taskObservedInfo)
        {
            return taskObservedInfo.IsFailed
                   && taskObservedInfo.CanContinue
                   && taskObservedInfo.Task.RunCount < FailedTaskRerunCount;
        }

        private void AddTaskWithDeelay(Task task)
        {
            BeforeRerunTask(task);
            var deelayTimer = new Timer(TimeSpan.FromSeconds(FailedTaskRerunPauseSeconds).TotalMilliseconds);
            deelayTimer.Elapsed += delegate
            {
                Add(task);
                deelayTimer.Stop();
                deelayTimer.Dispose();
            };
            deelayTimer.Start();
        }

        protected virtual void BeforeRerunTask(Task task)
        {
        }

        private void CheckForTasks()
        {
            if (tasks.Count != 0) return;
            taskRunEvent.Reset();
        }

        #endregion

        #region Flow States

        public bool IsStarted
        {
            get { return (thread.ThreadState & ThreadState.Unstarted) != ThreadState.Unstarted; }
        }

        public bool IsPaused
        {
            get { return (thread.ThreadState & ThreadState.WaitSleepJoin) == ThreadState.WaitSleepJoin; }
        }

        public bool IsRunning
        {
            get { return (thread.ThreadState & ThreadState.Running) == ThreadState.Running; }
        }

        #endregion

        #region Flow Control

        public void Start()
        {
            if (!IsStarted)
                thread.Start();

            if (!taskRunEvent.WaitOne(0) && tasks.Count > 0)
                taskRunEvent.Set();
        }

        public void Pause()
        {
            if (!IsRunning) return;

            taskRunEvent.Reset();
        }

        public void Cancell()
        {
            if (!IsStarted) return;

            if (!taskRunEvent.WaitOne(0))
            {
                taskCancelEvent.Set();
                taskRunEvent.Set();
            }

            thread.Join();
            taskRunEvent.Reset();
            thread = new Thread(TaskObserverProc);
        }

        protected T Sync<T>(Func<T> func)
        {
            T result;

            if (IsRunning)
            {
                Pause();
                taskSyncEvent.WaitOne();
                result = func();
                Start();
            }
            else
                result = func();

            return result;
        }

        protected void Sync(Action action)
        {
            if (IsRunning)
            {
                Pause();
                taskSyncEvent.WaitOne();
                action();
                Start();
            }
            else
                action();
        }

        #endregion

        #region Adding

        public void Add(Task task)
        {
            Add(new[] {task});
        }

        public void Add(IEnumerable<Task> newTasks)
        {
            Sync(() =>
            {
                newTasks = BeforeQueue(newTasks);

                var taskArray = newTasks as Task[] ?? newTasks.ToArray();

                foreach (var task in taskArray)
                    tasks.Enqueue(task);

                AfterQueue(taskArray);

                taskRunEvent.Set();
            });
        }

        protected virtual IEnumerable<Task> BeforeQueue(IEnumerable<Task> newTasks)
        {
            return newTasks;
        }

        protected virtual void AfterQueue(IEnumerable<Task> newTasks)
        {
        }

        #endregion

        public virtual void Dispose()
        {
            Cancell();
            taskRunEvent.Dispose();
            taskCancelEvent.Dispose();
            taskSyncEvent.Dispose();
        }
    }
}