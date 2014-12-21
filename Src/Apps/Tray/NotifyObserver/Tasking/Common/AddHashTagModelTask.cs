using System;
using MMK.Marking.Representation;

namespace MMK.Notify.Observer.Tasking.Common
{
    [Serializable]
    public sealed class AddHashTagModelTask : ChangeHashTagModelTask
    {
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
