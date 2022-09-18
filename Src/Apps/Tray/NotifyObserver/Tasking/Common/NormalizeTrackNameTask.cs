using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using MMK.Marking;
using MMK.Marking.Representation;
using MMK.Notify.Observer.Tasking.Common.Base;
using MMK.Notify.Observer.Tasking.Contexts;
using MMK.Processing;

namespace MMK.Notify.Observer.Tasking.Common
{
    [Serializable]
    public class NormalizeTrackNameTask : FileTask
    {
        public NormalizeTrackNameTask(string filePath) : base(filePath)
        {
        }

        public NormalizeTrackNameTask()
        {
        }

        private AudioFileContext AudioFileContext
        {
            get { return (AudioFileContext) FileContext; }
        }

        protected override void CheckContext()
        {
            base.CheckContext();
            if (!(FileContext is AudioFileContext))
                throw new InvalidTaskContextException("unsupported context type");
        }

        protected override INotifyable OnFileChange()
        {
            AudioFileContext.NameModel.HashTagModel = NormalizeHashTagModel(AudioFileContext.NameModel.HashTagModel);
            var tagger = new Id3Tager(AudioFileContext.FileInfo.FullName, AudioFileContext.NameModel);
            tagger.Tag();

            AudioFileContext.FileInfo.MoveTo(AudioFileContext.NormalizedFilePath);

            return new NotifyMessage
            {
                CommonDescription = "Normalize",
                TargetObject = AudioFileContext.ToString(),
                Type = NotifyType.Success
            };
        }

        private static HashTagModel NormalizeHashTagModel(HashTagModel hashTagModel)
        {
            if (hashTagModel.Contains(KeyHashTag.Unchecked) && hashTagModel.OfType<KeyHashTag>().Any())
                hashTagModel -= KeyHashTag.Unchecked;
            return hashTagModel;
        }

        public static IEnumerable<Task> Many(IEnumerable<string> paths)
        {
            if (paths == null)
                throw new ArgumentNullException("paths");
            Contract.EndContractBlock();

            return paths.Distinct().Select(p => new NormalizeTrackNameTask(p));
        }
    }
}