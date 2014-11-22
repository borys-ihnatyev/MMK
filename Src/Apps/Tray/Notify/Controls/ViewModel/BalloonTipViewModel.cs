using MMK.Notify.Observer;
using MMK.Wpf.ViewModel;

namespace MMK.Notify.Controls.ViewModel
{
    public class BalloonTipViewModel : ObservableObject
    {
        public BalloonTipViewModel()
        {
            NotifyType = NotifyType.Success;
        }

        private NotifyType notifyType;
        private string title;
        private string details;
        private string targetObject;

        public NotifyType NotifyType
        {
            get { return notifyType; }
            set
            {
                if (value != notifyType)
                {
                    notifyType = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string Title
        {
            get { return title; }
            set
            {
                if (value == title) return;
                
                title = value;
                NotifyPropertyChanged();
            }
        }

        public string Details
        {
            get { return details; }
            set
            {
                if (value == details) return;
                
                details = value;
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
