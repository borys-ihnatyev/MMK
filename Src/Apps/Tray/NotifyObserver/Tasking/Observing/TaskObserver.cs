using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using Timer = System.Timers.Timer;

namespace MMK.Notify.Observer.Tasking.Observing
{
    public sealed partial class TaskObserver : IDisposable
    {
        public const int FailedTaskRerunPauseSeconds = 5;

        private readonly ConcurrentQueue<Task> tasks;
        private readonly ManualResetEvent taskRunEvent;
        private readonly AutoResetEvent taskCancelEvent;
        private Thread thread;

        public TaskObserver()
        {
            tasks = new ConcurrentQueue<Task>();
            taskRunEvent = new ManualResetEvent(false);
            taskCancelEvent = new AutoResetEvent(false);
            thread = new Thread(TaskObserverProc);
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
            {
            }
        }

        private void ObserveTask(Task task)
        {
            var observedInfo = task.Run();

            if (observedInfo.IsNeedRerun)
            {
                observedInfo.MarkRerun();
                AddTaskWithDeelay(observedInfo.Task);
                return;
            }
            
            if (observedInfo.IsRerunFailed)
                observedInfo.UnmarkRerun();
            

            OnTaskObserved(observedInfo);
        }

        private void AddTaskWithDeelay(Task task)
        {
            var deelayTimer = CreateDeelayTimer();
            deelayTimer.Elapsed += delegate
            {
                Add(task); deelayTimer.Stop(); deelayTimer.Dispose();
            };
            deelayTimer.Start();
        }

        private static Timer CreateDeelayTimer()
        {
            return new Timer
            {
                Interval = TimeSpan.FromSeconds(FailedTaskRerunPauseSeconds).TotalMilliseconds,
                Enabled = false
            };
        }

        private void CheckForTasks()
        {
            lock (tasks)
            {
                if (tasks.Count == 0)
                {
                    taskRunEvent.Reset();
                    OnQueueEmpty();
                }
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
            var queuedTaskCount = newTasks.Count(task =>
            {
                tasks.Enqueue(task);
                return true;
            });

            OnTaskQueued(queuedTaskCount);

            lock (tasks)
                taskRunEvent.Set();
        }


        #endregion

        public void Dispose()
        {
            Cancell();
            taskRunEvent.Dispose();
            taskCancelEvent.Dispose();
        }

        #region Events

        public event EventHandler<TaskQueuedEventArgs> TaskQueued;
        public event EventHandler<EventArgs> QueueEmpty;

        public event EventHandler<NotifyEventArgs> TaskDone;
        public event EventHandler<NotifyEventArgs> TaskObserved;
        public event EventHandler<NotifyEventArgs> TaskFailed;

        private void OnQueueEmpty()
        {
            var handler = QueueEmpty;
            if (handler != null)
                handler(this, new EventArgs(this));
        }

        private void OnTaskQueued(int taskCount)
        {
            var handler = TaskQueued;
            if (handler != null)
                handler(this, new TaskQueuedEventArgs(this, taskCount));
        }

        private void OnTaskObserved(Task.ObservedInfo info)
        {
            Contract.Requires(info != null);

            OnNotifyEvent(TaskObserved, info);
            OnNotifyEvent(info.IsOk ? TaskDone : TaskFailed, info);
        }

        private void OnNotifyEvent(EventHandler<NotifyEventArgs> handler, INotifyable message)
        {
            if (handler != null)
                handler(this, new NotifyEventArgs(this, message));
        }

        #endregion
    }
}