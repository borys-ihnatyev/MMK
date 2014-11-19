using MMK.Marking.Representation;

namespace MMK.Notify.Observer.Tasking.Common
{
    internal sealed class RewriteHashTagModelTask : ChangeHashTagModelTask
    {
        public RewriteHashTagModelTask(string oldPath, HashTagModel hashTags) : base(oldPath, hashTags, null)
        {

        }

        protected override HashTagModel MakeNewHashTagModel()
        {
            return AddHashTagModel;
        }
    }
}
