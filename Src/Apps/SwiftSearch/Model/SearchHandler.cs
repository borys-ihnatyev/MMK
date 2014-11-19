namespace MMK.SwiftSearch.Model
{
    public abstract class SearchHandler
    {
        protected SearchHandler(string searchModel)
        {
            SearchModel = searchModel.Trim();
        }

        public string SearchModel
        {
            get; private set;
        }

        public abstract string IdentifyerTag { get; }

        public bool IsMatch()
        {
            return SearchModel.Contains(IdentifyerTag);
        }

        public string GetPureSearch()
        {
            return GetPureSearch(SearchModel);
        }

        public virtual string GetPureSearch(string search)
        {
            if (!string.IsNullOrWhiteSpace(IdentifyerTag))
                search = search.Replace(IdentifyerTag, "");
            return search;
        }

        public void Search()
        {
            OnSearch(GetPureSearch());
        }

        internal protected void Search(string pureSearch)
        {
            OnSearch(GetPureSearch(pureSearch));
        }

        protected abstract void OnSearch(string pureSearch);
    }
}
