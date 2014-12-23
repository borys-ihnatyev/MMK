using System.Collections.Generic;
using MMK.ApplicationServiceModel;
using MMK.Marking;
using MMK.Notify.Observer;
using MMK.Notify.Observer.Tasking.Common;

namespace MMK.HotMark.Model
{
    public sealed class RewriteHashTagModel : HashTagModelChangeModel
    {
        public RewriteHashTagModel(IEnumerable<string> paths, IEnumerable<HashTag> initialConjointHashTagModel)
            : base(paths, initialConjointHashTagModel)
        { }

        protected override void OnNotifyChange()
        {
            var notifyObserver = IoC.ServiceLocator.Get<INotifyObserver>();
            notifyObserver.Observe(RewriteHashTagModelTask.Many(Paths, FinalHashTagModel));
        }
    }
}