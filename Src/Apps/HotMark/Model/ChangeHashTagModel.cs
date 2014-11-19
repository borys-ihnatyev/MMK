using System.Collections.Generic;
using System.Windows;
using MMK.Marking;
using MMK.Marking.Representation;
using MMK.Notify.Observer;

namespace MMK.HotMark.Model
{
    public sealed class ChangeHashTagModel : HashTagModelChangeNotify
    {
        public ChangeHashTagModel(IEnumerable<string> paths, IEnumerable<HashTag> initialConjointHashTagModel)
            : base(paths, initialConjointHashTagModel)
        {
        }

        protected override void OnNotifyChange()
        {
            var addHashTagModel = new HashTagModel(FinalHashTagModel);
            var removeHashTagModel = new HashTagModel(InitialConjointHashTagModel);

            var notChangedHashTags = new HashTagModel(InitialConjointHashTagModel);
            notChangedHashTags.IntersectWith(FinalHashTagModel);

            addHashTagModel -= notChangedHashTags;
            removeHashTagModel -= notChangedHashTags;

            if(addHashTagModel.Count == 0 && removeHashTagModel.Count == 0)
                return;

            var notifyObserverOwner = Application.Current as INotifyObserverOwner;
            if (notifyObserverOwner == null) return;

            if (removeHashTagModel.Count == 0 && addHashTagModel.Count > 0)
                notifyObserverOwner.NotifyObserver.AddHashTagModel(Paths, addHashTagModel);
            else
                notifyObserverOwner.NotifyObserver.ChangeHashTagModel(Paths, addHashTagModel, removeHashTagModel);
        }
    }
}