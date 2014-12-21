using System;
using System.Collections.Generic;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using MMK.Notify.Observer.Tasking;
using MMK.Notify.Observer.Tasking.Observing;

namespace MMK.Notify.Observer.Remoting
{
    public class NotifyObserver : MarshalByRefObject, INotifyObserver
    {
        private static NotifyObserver instance;

        private readonly TaskObserver observer;
        private IpcChannel channel;

        /// <summary>
        ///     Only for remoting usage
        /// </summary>
        public NotifyObserver() : this(null)
        {
        }

        public NotifyObserver(TaskObserver taskObserver)
        {
            if (instance == null)
                instance = this;

            IsStarted = false;
            observer = taskObserver;
        }

        public bool IsStarted { get; private set; }

        public void Start()
        {
            if (IsStarted)
                return;

            channel = new IpcChannel(NotifyObserverRemotingInfo.IpcChannelPortName);
            ChannelServices.RegisterChannel(channel, false);
            RemotingConfiguration.RegisterWellKnownServiceType(
                typeof (NotifyObserver),
                NotifyObserverRemotingInfo.MessageObjectFileName,
                WellKnownObjectMode.Singleton
                );
            IsStarted = true;
        }

        public bool TryStart()
        {
            try
            {
                Start();
            }
            catch (RemotingException)
            {
                IsStarted = false;
            }

            return IsStarted;
        }

        public void Observe(Task task)
        {
            observer.Add(task);
        }

        public void Observe(IEnumerable<Task> tasks)
        {
            observer.Add(tasks);
        }

        public void Stop()
        {
            if (!IsStarted)
                return;

            ChannelServices.UnregisterChannel(channel);

            IsStarted = false;
        }

        void INotifyObserver.Observe(Task task)
        {
            instance.Observe(task);
        }

        void INotifyObserver.Observe(IEnumerable<Task> tasks)
        {
            instance.Observe(tasks);
        }

        void INotifyObserver.TestConnection()
        {
        }
    }
}