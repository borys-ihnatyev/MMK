using System.Collections.Generic;
using System.Linq;
using MMK.Marking.Representation;

namespace MMK.Notify.Observer.Tasking.Common
{
    public sealed class RewriteHashTagModelTask : ChangeHashTagModelTask
    {
        public static IEnumerable<Task> Many(IEnumerable<string> paths, HashTagModel newHashTagModel)
        {
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
