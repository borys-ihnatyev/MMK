using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Media;
using MMK.Taging;
using MMK.Wpf.ViewModel;

namespace MMK.HashTagSearch.ViewModel
{
    public class SearchNavigationViewModel : ViewModelBase
    {
        public SearchNavigationViewModel()
        {
            StyleHashTags = new ObservableCollection<string>();
            KeyHashTags = new ObservableCollection<ParalelKeysViewModel>();
        }

        public ObservableCollection<string> StyleHashTags { get; private set; }

        public ObservableCollection<ParalelKeysViewModel> KeyHashTags { get; private set; }

        protected override void OnLoadData()
        {
            foreach (var key in CircleOfFifths.MinorKeys)
            {
                KeyHashTags.Add(new ParalelKeysViewModel(CircleOfFifths.GetParalel(key), key));
            }

            StyleHashTags.Add("#pop");
            StyleHashTags.Add("#house");
            StyleHashTags.Add("#rnb");
            StyleHashTags.Add("#slow");
            StyleHashTags.Add("#old");
            StyleHashTags.Add("#new");
            StyleHashTags.Add("#unch");    
        }
    }
}
