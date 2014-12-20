using MMK.Notify.Observer;
using MMK.Wpf.ViewModel;

namespace MMK.Notify.ViewModel
{
    public class BalloonTipViewModel : ObservableObject, INotifyable
    {
        public BalloonTipViewModel()
        {
            Type = NotifyType.Success;
        }

        public BalloonTipViewModel(INotifyable notifyable)
        {
            Type = notifyable.Type;
            CommonDescription = notifyable.CommonDescription;
            DetailedDescription = notifyable.DetailedDescription;
            TargetObject = notifyable.TargetObject;
        }

        private NotifyType type;
        private string commonDescription;
        private string detailedDescription;
        private string targetObject;

        public NotifyType Type
        {
            get { return type; }
            set
            {
                if (value == type) return;
                type = value;
                NotifyPropertyChanged();
            }
        }

        public string CommonDescription
        {
            get { return commonDescription; }
            set
            {
                if (value == commonDescription) return;
                
                commonDescription = value;
                NotifyPropertyChanged();
            }
        }

        public string DetailedDescription
        {
            get { return detailedDescription; }
            set
            {
                if (value == detailedDescription) return;
                
                detailedDescription = value;
                NotifyPropertyChanged();
            }
        }

        public string TargetObject
        {
            get { return targetObject; }
            set
            {
                if (value == targetObject) return;
                
                targetObject = value;
                NotifyPropertyChanged();
            }
        }
    }
}
