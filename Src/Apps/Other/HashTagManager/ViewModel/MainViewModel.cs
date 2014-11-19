using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MMK.MusicDatabase;
using MMK.Wpf.ViewModel;

namespace MMK.HashTagManager.ViewModel
{
    public class MainViewModel : ViewModelBase, IDisposable
    {
        public MainViewModel()
        {
            HashTagItems = new ObservableCollection<HashTagItemViewModel>();
            ///TODO
            ///dbContext = new Context();
        }

        //TODO 
        //private Context dbContext;

        public ObservableCollection<HashTagItemViewModel> HashTagItems { get; private set; }

        protected override void OnLoadData()
        {
            /// UNDONE : add items from dbContext to HashTagItems
        }

        public void Dispose()
        {
            /// TODO : add EntityFramework reference and uncoment next line
            /// dbContext.Dispose();
        }
    }
}
