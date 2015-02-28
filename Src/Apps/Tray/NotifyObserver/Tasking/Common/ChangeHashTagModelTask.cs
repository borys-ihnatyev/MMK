using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using MMK.Marking.Representation;
using MMK.Notify.Observer.Tasking.Common.Base;
using MMK.Notify.Observer.Tasking.Contexts;
using MMK.Processing;

namespace MMK.Notify.Observer.Tasking.Common
{
    public class ChangeHashTagModelTask : FileTask
    {
        protected readonly HashTagModel Add;
        protected readonly HashTagModel Remove;


        public ChangeHashTagModelTask(string filePath, HashTagModel add, HashTagModel remove)
            : base(filePath)
        {
            Add = add;
            Remove = remove;
        }

        public ChangeHashTagModelTask(HashTagModel add, HashTagModel remove)
        {
            Add = add;
            Remove = remove;
        }

        sealed protected override INotifyable OnFileChange()
        {
            var audioFileContext = FileContext as AudioFileContext;
            if (audioFileContext != null)
                OnAudioFileChange(audioFileContext);
            else
                OnFileChange(FileContext);

            return new NotifyMessage
            {
                CommonDescription = "Change hash tags",
                TargetObject = Context.ToString()
            };
        }

        private void OnAudioFileChange(AudioFileContext context)
        {
            context.NameModel.HashTagModel = ChangeHashTagModel(context.NameModel.HashTagModel);

            if (context.IsNormalizedFileName)
                throw new Cancel("file name not changed");

            var tager = new Id3Tager(context.FileInfo.FullName, context.NameModel);
            tager.Tag();
            context.FileInfo.MoveTo(context.NormalizedFilePath);
        }

        private void OnFileChange(FileContext context)
        {
            var hashTagModel = ChangeHashTagModel(context.FileHashTagModel);

            var newFilePath = Path.Combine(
                context.FileInfo.DirectoryName,
                context.FileNameWithoutHashTagModel + " " + hashTagModel + context.FileInfo.Extension
                );

            if (context.FileInfo.FullName.Equals(newFilePath, StringComparison.OrdinalIgnoreCase))
                throw new Cancel("file name not changed");

            context.FileInfo.MoveTo(newFilePath);
        }

        protected virtual HashTagModel ChangeHashTagModel(HashTagModel hashTagModel)
        {
            hashTagModel -= Remove;
            hashTagModel += Add;
            return hashTagModel;
        }

        public static IEnumerable<Task> Many(IEnumerable<string> paths, HashTagModel add, HashTagModel remove)
        {
            if (paths == null)
                throw new ArgumentNullException("paths");
            Contract.EndContractBlock();

            return paths.Distinct().Select(p => new ChangeHashTagModelTask(p, add, remove));
        }
    }
}