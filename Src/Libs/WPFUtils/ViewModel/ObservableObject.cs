using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MMK.Wpf.ViewModel
{
    public abstract class ObservableObject : INotifyPropertyChanged 
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged([CallerMemberName]string propertyName = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
