using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using MMK.Marking.Representation;

namespace MMK.Notify.Observer.Tasking.Common
{
    public sealed class RewriteHashTagModelTask : ChangeHashTagModelTask
    {
        public static IEnumerable<Task> Many(IEnumerable<string> paths, HashTagModel newHashTagModel)
        {
            if (paths == null)
                throw new ArgumentNullException("paths");
            if(newHashTagModel == null)
                throw new ArgumentNullException("newHashTagModel");
            Contract.EndContractBlock();

            return paths.Select(p => new RewriteHashTagModelTask(p, newHashTagModel));
        }  


        public RewriteHashTagModelTask(string oldPath, HashTagModel hashTags) : base(oldPath, hashTags, null)
        { }

        protected override HashTagModel MakeNewHashTagModel()
        {
            return AddHashTagModel;
        }
    }
}
