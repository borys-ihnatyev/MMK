using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Windows.Input;
using MMK.ApplicationServiceModel;
using MMK.Presentation.Providers;
using MMK.Presentation.Providers.Key;
using MMK.Presentation.ViewModel;
using MMK.SwiftSearch.Properties;
using MMK.SwiftSearch.SearchHandlers;
using MMK.Utils.Extensions;

namespace MMK.SwiftSearch.ViewModels
{
    public class SwiftSearchViewModel : ViewModel
    {
        private readonly Dictionary<string, string> searchHandlerNameIcon = new Dictionary<string, string>
        {
            {"", "\uf002"},
            {"g", "\uf1a0"},
            {"vk", "\uf189"},
            {"zn", "\uf019"}
        };

        private readonly MusicalKeyGlobalShortcutProvider shortcutProvider;

        private string search;
        private string searchHandlerIconText;
        private string searchHandlerName;

        public SwiftSearchViewModel() : this(String.Empty)
        {
        }

        public SwiftSearchViewModel(string search)
        {
            Search = search.RemoveNewLines().Trim();
            SetSearchHandler();
            shortcutProvider = new MusicalKeyGlobalShortcutProvider();
            shortcutProvider.HotKeyPressed += AddSearchParalel;
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
                if (this.IsCalledInside(2))
                    OnSearchSelfChanged();
            }
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

        private void AddSearchParalel(Key key)
        {
            var keyStr = string.Format("<#{0}|#{1}>", key, CircleOfFifths.GetParalel(key));
            AddSearch(keyStr);
        }

        protected override void OnLoadData()
        {
            IoC.Get<GlobalShortcutProviderCollection>().Add(shortcutProvider);
            shortcutProvider.StartListen();
        }

        protected override void OnUnloadData()
        {
            shortcutProvider.StopListen();
            IoC.Get<GlobalShortcutProviderCollection>().Remove(shortcutProvider);
        }


        public ICommand ToggleSearchCommand { get; private set; }

        public void ToggleSearch()
        {
            SearchToggle = Search;
            Search = String.Empty;
        }

        public ICommand AddSearchCommand { get; private set; }

        public void AddSearch(string word)
        {
            Search = String.Format("{0} {1} ", Search, word.Trim()).TrimStart();
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
        }

        public event EventHandler SearchSelfChanged;

        private void OnSearchSelfChanged()
        {
            var handler = SearchSelfChanged;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }
    }
}