using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using MMK.SwiftSearch.Properties;
using MMK.SwiftSearch.SearchHandlers;
using MMK.SwiftSearch.Views;
using MMK.Utils.Extensions;
using MMK.Wpf.Providers;
using MMK.Wpf.Providers.Key;
using MMK.Wpf.ViewModel;

namespace MMK.SwiftSearch.ViewModels
{
    public class SwiftSearchViewModel : ViewModel
    {
        private string searchHandlerName;
        private string search;
        private string searchHandlerIconText;

        private readonly GlobalKeyShortcutProvider keyShortcutProvider;
        private readonly Dictionary<string, string> searchHandlerNameIcon = new Dictionary<string, string>
        {
            {"", "\uf002"},
            {"g", "\uf1a0"},
            {"vk", "\uf189"},
            {"zn", "\uf019"}
        };

        public SwiftSearchViewModel() : this(String.Empty)
        {
        }

        public SwiftSearchViewModel(string search)
        {
            Search = search.RemoveNewLines().Trim();
            SetSearchHandler();
            keyShortcutProvider = new GlobalKeyShortcutProvider();
            keyShortcutProvider.HotKeyPressed += AddSearchParalelKeys;
        }


        public string SearchHandlerIconText
        {
            get { return searchHandlerIconText; }
            private set
            {
                if (value == searchHandlerIconText) return;
                searchHandlerIconText = value;
                NotifyPropertyChanged();
            }
        }

        public string Search
        {
            get { return search; }
            set
            {
                value = value.RemoveNewLines();

                if (value == search) return;
                search = value;

                NotifyPropertyChanged();
                if(this.IsCalledInside(2))
                    OnSearchSelfChanged();
            }
        }

        private void OnSearchSelfChanged()
        {
            var handler = SearchSelfChanged;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        public string SearchToggle
        {
            get { return Settings.Default.SearchToggle; }
            set
            {
                value = value.RemoveNewLines().Trim();
                if (String.Equals(value, Settings.Default.SearchToggle, StringComparison.Ordinal))
                    return;
                Settings.Default.SearchToggle = value;
                NotifyPropertyChanged();
            }
        }

        public string FullSearch
        {
            get { return String.Format("{0} {1}", SearchToggle, Search).Trim(); }
        }


        protected override void OnLoadData()
        {
            (keyShortcutProvider as IGlobalShortcutProvider).SetWindow(GetView());
            keyShortcutProvider.StartListen();
        }

        protected override void OnUnloadData()
        {
            keyShortcutProvider.StopListen();
        }

        private void AddSearchParalelKeys(Key key)
        {
            var keyStr = string.Format("<#{0}|#{1}>", key, CircleOfFifths.GetParalel(key));
            AddSearch(keyStr);
        }


        public ICommand ToggleSearchCommand { get; private set; }

        public void ToggleSearch()
        {
            if (String.IsNullOrWhiteSpace(SearchToggle))
            {
                SearchToggle = Search;
                Search = String.Empty;
            }
            else
            {
                Search += " " + SearchToggle;
                SearchToggle = String.Empty;
            }
        }


        public ICommand AddSearchCommand { get; private set; }

        public void AddSearch(string word)
        {
            Search = String.Format("{0} {1}", Search, word);
        }


        public ICommand SetSearchHandlerCommand { get; private set; }

        public void SetSearchHandler(string name = "")
        {
            if (!searchHandlerNameIcon.ContainsKey(name))
                throw new ArgumentException(String.Format("No such search handlerName \"{0}\"", name), "name");
            Contract.EndContractBlock();

            SearchHandlerIconText = searchHandlerNameIcon[name];
            searchHandlerName = name;
        }


        public ICommand ApplySearchCommand { get; private set; }

        public void ApplySearch()
        {
            var searchHandler = SearchHandlerFactory.Create(searchHandlerName, FullSearch);
            searchHandler.Search();
            CloseView();
        }

        public ICommand CancelCommand { get; private set; }

        public void Cancel()
        {
            CloseView();
        }

        public event EventHandler SearchSelfChanged;

        private static void CloseView()
        {
            var window = GetView();
            window.Close();
        }

        private static Window GetView()
        {
            Contract.Ensures(Contract.Result<Window>() != null);
            Contract.EndContractBlock();
            return Application.Current.Windows.OfType<SwiftSearchView>().FirstOrDefault();
        }
    }
}