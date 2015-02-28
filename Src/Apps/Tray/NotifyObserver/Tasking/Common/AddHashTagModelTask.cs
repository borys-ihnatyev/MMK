using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using MMK.Marking.Representation;

namespace MMK.Notify.Observer.Tasking.Common
{
    public sealed class AddHashTagModelTask : ChangeHashTagModelTask
    {
        public AddHashTagModelTask(string filePath, HashTagModel add) : base(filePath, add, null)
        {
        }

        public AddHashTagModelTask(HashTagModel add) : base(add, null)
        {
        }

        protected override HashTagModel ChangeHashTagModel(HashTagModel hashTagModel)
        {
            hashTagModel += Add;
            return hashTagModel;
        }

        public static IEnumerable<Task> Many(IEnumerable<string> paths, HashTagModel add)
        {
            if (paths == null)
                throw new ArgumentNullException("paths");
            if (add == null)
                throw new ArgumentNullException("add");
            Contract.EndContractBlock();
            return paths.Distinct().Select(p => new AddHashTagModelTask(p, add));
        }
    }
}