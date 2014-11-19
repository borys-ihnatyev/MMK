using System;
using System.Runtime.Serialization;
using MMK.Marking.Representation;
using MMK.Processing;
using MMK.Utils;

namespace MMK.Notify.Observer.Tasking.Common.Base
{
    [Serializable]
    internal abstract class Mp3FileChangeTask : FileChangeTask
    {
        protected Mp3FileChangeTask(string oldPath) : base(oldPath)
        {
        }

        protected Mp3FileChangeTask(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }

        protected override void Initialize()
        {
            base.Initialize();

            if (!FileExtensionParser.IsMp3(OldFile.Extension))
                ThrowAsNotifyableException(new Exception("Файлы типа \"" + OldFile.Extension + "\" не поддерживаются"));

            NameModel = TrackNameModel.Parser.Parse(CleanName);
        }

        protected override void OnFileChange()
        {
            OnFileChangeSkipCancel();
            new Id3Tager(NewFile.FullName, NameModel).Tag();
        }

        private void OnFileChangeSkipCancel()
        {
            try
            {
                base.OnFileChange();
            }
            catch (Cancel)
            {
            }
        }

        public TrackNameModel NameModel { get; private set; }
    }
}