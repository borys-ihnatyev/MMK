using System.Collections.Generic;
using System.Threading;
using System.Windows;
using MMK.ApplicationServiceModel;
using MMK.Marking;
using MMK.Marking.Representation;
using MMK.Notify.Observer;
using MMK.Notify.Observer.Tasking;
using MMK.Notify.Observer.Tasking.Common;

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

            var notifyObserver = IoC.ServiceLocator.Get<INotifyObserver>();

            IEnumerable<Task> tasks;
            
            if (removeHashTagModel.Count == 0 && addHashTagModel.Count > 0)
                tasks = AddHashTagModelTask.Many(Paths, addHashTagModel);
            else
                tasks = ChangeHashTagModelTask.Many(Paths, addHashTagModel, removeHashTagModel);

            notifyObserver.Observe(tasks);
        }
    }
}