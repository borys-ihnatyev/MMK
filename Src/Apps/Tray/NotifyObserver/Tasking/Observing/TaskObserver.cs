using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using Timer = System.Timers.Timer;

namespace MMK.Notify.Observer.Tasking.Observing
{
    public class TaskObserver : IDisposable
    {
        public const int FailedTaskRerunPauseSeconds = 5;

        private readonly ManualResetEvent taskRunEvent;
        private readonly AutoResetEvent taskCancelEvent;

        public event Action<INotifyable> TaskDone;
        public event Action<INotifyable> TaskObserved;
        public event Action<INotifyable> TaskFailed;

        public event Action<INotifyable> Notify;

        private Thread thread;

        private readonly ConcurrentQueue<Task> tasks;

        public TaskObserver()
        {
            tasks = new ConcurrentQueue<Task>();

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


        protected virtual void OnNotify(INotifyable notifyable)
        {
            var handler = Notify;
            if (handler != null) 
                handler(notifyable);
        }

        
        #region Flow

        private void TaskObserverProc()
        {
            while (!taskCancelEvent.WaitOne(0))
            {
                taskRunEvent.WaitOne();
                Task task;
                if (tasks.TryDequeue(out task))
                    TryObserveTask(task);
                CheckForTasks();
            }
        }

        private void TryObserveTask(Task task)
        {
            try
            {
                ObserveTask(task);
            }
            catch (Task.Cancel)
            { }
        }

        private void ObserveTask(Task task)
        {
            var observedInfo = task.Run();

            if (observedInfo.IsNeedRerun)
            {
                observedInfo.MarkRerun();
                AddTaskWithDeelay(observedInfo.Task);

                if (task.RunCount < 1)
                    return;
            }
            else {
                if (observedInfo.IsRerunFailed)
                    observedInfo.UnmarkRerun();
            }

            OnTaskObserved(observedInfo);
        }

        private void AddTaskWithDeelay(Task task)
        {
            var deelayTimer = CreateDeelayTimer();
            deelayTimer.Elapsed += delegate{ Add(task); };
            deelayTimer.Start();
        }

        private static Timer CreateDeelayTimer()
        {
            return new Timer
            {
                Interval = TimeSpan.FromSeconds(FailedTaskRerunPauseSeconds).TotalMilliseconds,
                AutoReset = true,
                Enabled = false
            };
        }

        private void CheckForTasks()
        {
            lock (tasks)
            {
                if (tasks.Count == 0)
                    taskRunEvent.Reset();
            }
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

        #endregion

        #region Adding

        public void Add(Task task)
        {
            Add(new[] {task});
        }

        public void Add(IEnumerable<Task> newTasks)
        {
            newTasks = BeforeQueue(newTasks);

            var taskArray = newTasks as Task[] ?? newTasks.ToArray();

            foreach (var task in taskArray)
                tasks.Enqueue(task);

            AfterQueue(taskArray);

            lock (tasks)
                taskRunEvent.Set();
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
        }
    }
}