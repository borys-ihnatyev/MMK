using System.Collections.Generic;
using System.Linq;
using MMK.SwiftSearch.Model;

namespace MMK.SwiftSearch.SearchHandlers
{
    sealed public class MultiSearchHandler : SearchHandler
    {
        private readonly SearchHandler defaultHandler;
        private readonly LinkedList<SearchHandler> searchHandlers;
        private List<SearchHandler> matchedHandlers; 

        public MultiSearchHandler(string searchModel) : base(searchModel)
        {
            defaultHandler = new EverythingSearchHandler(searchModel);

            searchHandlers = new LinkedList<SearchHandler>();
            searchHandlers.AddLast(new VkSearchHandler(searchModel));
            searchHandlers.AddLast(new GoodleSearchHandler(searchModel));
            searchHandlers.AddLast(new ZaycevNetSearchHandler(searchModel));
            searchHandlers.AddLast(defaultHandler);
        }

        public override string IdentifyerTag
        {
            get { return ""; }
        }

        protected override void OnSearch(string pureSearch)
        {
            if (matchedHandlers.Count == 0)
                matchedHandlers.Add(defaultHandler);

            foreach (var handler in matchedHandlers)
                handler.Search(pureSearch);
        }

        private List<SearchHandler> GetMatchedHandlers()
        {
            return searchHandlers.Where(h => h.IsMatch()).ToList();
        }

        public override string GetPureSearch(string search)
        {
            matchedHandlers = GetMatchedHandlers();
            return matchedHandlers.Aggregate(search, (s, handler) => handler.GetPureSearch(s));
        }
    }
}
