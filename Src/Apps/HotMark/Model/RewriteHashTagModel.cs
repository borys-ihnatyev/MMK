using System.Collections.Generic;
using System.Windows;
using MMK.Marking;
using MMK.Notify.Observer;

namespace MMK.HotMark.Model
{
    public sealed class RewriteHashTagModel : HashTagModelChangeNotify
    {
        public RewriteHashTagModel(IEnumerable<string> paths, IEnumerable<HashTag> initialConjointHashTagModel) : base(paths, initialConjointHashTagModel)
        {

        }

        protected override void OnNotifyChange()
        {
            var notifyObserverOwner = Application.Current as INotifyObserverOwner;
            if (notifyObserverOwner != null)
                notifyObserverOwner.NotifyObserver.RewriteHashTagModel(Paths, FinalHashTagModel);
        }
    }
}
