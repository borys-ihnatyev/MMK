using MMK.SwiftSearch.Model;

namespace MMK.SwiftSearch.SearchHandlers
{
    sealed class GoodleSearchHandler : WebSearchHandler
    {
        public GoodleSearchHandler(string searchModel) : base(searchModel)
        {

        }

        protected override string UrlFormat
        {
            get { return "http://www.google.com/search?q={0}"; }
        }
    }
}
