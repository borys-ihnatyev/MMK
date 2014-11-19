using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using MMK.Wpf.ViewModel;

namespace MMK.HashTagSearch.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel()
        {
            SearchContent = new SearchContentViewModel();
            SearchNavigation = new SearchNavigationViewModel();
        }

        public SearchNavigationViewModel SearchNavigation { get; private set; }

        public SearchContentViewModel SearchContent { get; private set; }
    }
}
