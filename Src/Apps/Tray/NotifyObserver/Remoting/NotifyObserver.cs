using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using MMK.Marking.Representation;
using MMK.Notify.Observer.Tasking;
using MMK.Notify.Observer.Tasking.Common;
using MMK.Notify.Observer.Tasking.Observing;
using MMK.Processing.AutoFolder;

namespace MMK.Notify.Observer.Remoting
{
    public class NotifyObserver : MarshalByRefObject, INotifyObserver
    {
        private static NotifyObserver instance;

        private readonly TaskObserver observer;

        /// <summary>
        /// Only for remoting usage
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
            if (IsStarted) return;

            var channel = new IpcChannel(NotifyObserverRemotingInfo.IpcChannelPortName);
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


        public void ChangeHashTagModel(string path, HashTagModel add, HashTagModel remove)
        {
            observer.Add(new ChangeHashTagModelTask(path, add,remove));
        }

        public void ChangeHashTagModel(IEnumerable<string> paths, HashTagModel add, HashTagModel remove)
        {
            var pathsArray = paths as string[] ?? paths.ToArray();
            var tasks = new Task[pathsArray.Length];

            for (var i = 0; i < pathsArray.Length; i++)
                tasks[i] = new ChangeHashTagModelTask(pathsArray[i], add, remove);

            observer.Add(tasks);
        }


        public void RewriteHashTagModel(string path, HashTagModel hashTags)
        {
            observer.Add(new RewriteHashTagModelTask(path, hashTags));
        }

        public void RewriteHashTagModel(IEnumerable<string> paths, HashTagModel hashTags)
        {
            var pathsArray = paths as string[] ?? paths.ToArray();
            var tasks = new Task[pathsArray.Length];

            for (var i = 0; i < pathsArray.Length; i++)
                tasks[i] = new RewriteHashTagModelTask(pathsArray[i], hashTags);

            observer.Add(tasks);
        }


        public void AddHashTagModel(string path, HashTagModel hashTags)
        {
            observer.Add(new AddHashTagModelTask(path, hashTags));
        }

        public void AddHashTagModel(IEnumerable<string> paths, HashTagModel hashTags)
        {
            var pathsArray = paths as string[] ?? paths.ToArray();
            var tasks = new Task[pathsArray.Length];

            for (var i = 0; i < pathsArray.Length; i++)
                tasks[i] = new AddHashTagModelTask(pathsArray[i], hashTags);

            observer.Add(tasks);
        }


        public void NormalizeTrackName(string path)
        {
            observer.Add(new NormalizeTrackNameTask(path));
        }

        public void NormalizeTrackName(IEnumerable<string> paths)
        {
            var pathsArray = paths as string[] ?? paths.ToArray();
            var tasks = new Task[pathsArray.Length];


            for (var i = 0; i < pathsArray.Length; i++)
                tasks[i] = new NormalizeTrackNameTask(pathsArray[i]);

            observer.Add(tasks);
        }


        public void MoveToCollectionFolder(string path, HashTagFolderCollection collection)
        {
            observer.Add(new MoveFileToMusicFolderTask(path, collection));
        }

        public void MoveToCollectionFolder(IEnumerable<string> paths, HashTagFolderCollection collection)
        {
            var pathsArray = paths as string[] ?? paths.ToArray();
            var tasks = new Task[pathsArray.Length];

            for (var i = 0; i < pathsArray.Length; i++)
                tasks[i] = new MoveFileToMusicFolderTask(pathsArray[i],collection);

            observer.Add(tasks);
        }


        void INotifyObserver.ChangeHashTagModel(string path, HashTagModel add, HashTagModel remove)
        {
            instance.ChangeHashTagModel(path,add,remove); 
        }

        void INotifyObserver.ChangeHashTagModel(IEnumerable<string> paths, HashTagModel add, HashTagModel remove)
        {
            instance.ChangeHashTagModel(paths, add, remove);
        }

        void INotifyObserver.RewriteHashTagModel(string path, HashTagModel hashTags)
        {
            instance.RewriteHashTagModel(path, hashTags);
        }

        void INotifyObserver.RewriteHashTagModel(IEnumerable<string> paths, HashTagModel hashTags)
        {
            instance.RewriteHashTagModel(paths, hashTags);
        }

        void INotifyObserver.AddHashTagModel(string path, HashTagModel hashTags)
        {
            instance.AddHashTagModel(path, hashTags);
        }

        void INotifyObserver.AddHashTagModel(IEnumerable<string> paths, HashTagModel hashTags)
        {
            instance.AddHashTagModel(paths, hashTags);
        }


        void INotifyObserver.NormalizeTrackName(string path)
        {
            instance.NormalizeTrackName(path);
        }

        void INotifyObserver.NormalizeTrackName(IEnumerable<string> paths)
        {
            instance.NormalizeTrackName(paths);
        }


        void INotifyObserver.MoveToCollectionFolder(string path, HashTagFolderCollection collection)
        {
            instance.MoveToCollectionFolder(path, collection);
        }

        void INotifyObserver.MoveToCollectionFolder(IEnumerable<string> paths, HashTagFolderCollection collection)
        {
            instance.MoveToCollectionFolder(paths, collection);
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