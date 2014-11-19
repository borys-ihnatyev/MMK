using System;
using System.Diagnostics;

namespace MMK.SwiftSearch.Model
{
    public abstract class WebSearchHandler : SearchHandler
    {
        protected WebSearchHandler(string searchModel) : base(searchModel)
        {
        
        }

        protected abstract string UrlFormat { get; }

        sealed protected override void OnSearch(string pureSearch)
        {
            Process.Start(string.Format(UrlFormat, Uri.EscapeDataString(pureSearch)));
        }
    }
}
