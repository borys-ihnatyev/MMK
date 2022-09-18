using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using MMK.Marking.Representation;

namespace MMK.Notify.Observer.Tasking.Common
{
    public sealed class RewriteHashTagModelTask : ChangeHashTagModelTask
    {
        public RewriteHashTagModelTask(string filePath, HashTagModel hashTagModel)
            : base(filePath, hashTagModel, null)
        {
        }

        public RewriteHashTagModelTask(HashTagModel hashTagModel)
            : base(hashTagModel, null)
        {
        }

        protected override HashTagModel ChangeHashTagModel(HashTagModel hashTagModel)
        {
            return Add;
        }

        public static IEnumerable<Task> Many(IEnumerable<string> paths, HashTagModel hashTagModel)
        {
            if (paths == null)
                throw new ArgumentNullException("paths");
            if (hashTagModel == null)
                throw new ArgumentNullException("hashTagModel");
            Contract.EndContractBlock();

            return paths.Select(p => new RewriteHashTagModelTask(p, hashTagModel));
        }
    }
}