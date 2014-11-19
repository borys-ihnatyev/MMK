using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MMK.Search;
using MMK.Wpf.ViewModel;
using MMK.HashTagSearch.Model;

namespace MMK.HashTagSearch.ViewModel
{
    public class SearchContentViewModel : ViewModelBase
    {
        public SearchContentViewModel()
        {
            model = new SearchContentModel();
            Items = new ObservableCollection<ContentItemViewModel>();
        }

        private SearchContentModel model;
        private string searchString;

        public string SearchString
        {
            get { return searchString; }
            set
            {
                if (searchString != value)
                {
                    searchString = value;
                    OnSearchStringChanged();
                }
            }
        }

        public int ItemsCount { get; private set; }

        public ObservableCollection<ContentItemViewModel> Items { get; private set; }

        protected void OnSearchStringChanged()
        {
            NotifyPropertyChanged("SearchString");
            model.SearchString = searchString;
            UpdateItems();
        }

        protected override void OnLoadData()
        {
            model.SearchString = ".mp3";
        }

        private void UpdateItems() 
        {
            Items.Clear();
            foreach (var filePath in model.GetFiles())
                Items.Add(filePath);

            ItemsCount = Items.Count;
            NotifyPropertyChanged("ItemsCount");
        }
    }
}
