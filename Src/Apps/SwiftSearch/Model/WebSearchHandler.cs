using System;
using System.Diagnostics;
using MMK.Marking.Representation;

namespace MMK.SwiftSearch.Model
{
    public abstract class WebSearchHandler : ISearchHandler
    {
        protected WebSearchHandler(string searchModel)
        {
            HashTagModel.Parser.All(ref searchModel);
            SearchModel = Uri.EscapeDataString(searchModel.Trim());
        }

        protected abstract string UrlFormat { get; }

        public string SearchModel { get; private set; }

        public void Search()
        {
            Process.Start(string.Format(UrlFormat, SearchModel));
        }
    }
}
