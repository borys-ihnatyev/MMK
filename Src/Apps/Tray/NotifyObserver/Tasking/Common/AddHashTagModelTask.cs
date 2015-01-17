using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using MMK.Marking.Representation;

namespace MMK.Notify.Observer.Tasking.Common
{
    [Serializable]
    public sealed class AddHashTagModelTask : ChangeHashTagModelTask
    {
        public static IEnumerable<Task> Many(IEnumerable<string> paths, HashTagModel add)
        {
            if (paths == null)
                throw new ArgumentNullException("paths");
            if (add == null)
                throw new ArgumentNullException("add");
            Contract.EndContractBlock();
            return paths.Select(p => new AddHashTagModelTask(p, add));
        }  

        public AddHashTagModelTask(string oldPath, HashTagModel hashTags) 
            : base(oldPath, hashTags, null)
        {

        }

        protected override HashTagModel MakeNewHashTagModel()
        {
            return new HashTagModel(AddHashTagModel) + NameModel.HashTagModel;
        }
    }
}
