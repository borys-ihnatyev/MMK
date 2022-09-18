using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using MMK.SwiftSearch.Model;

namespace MMK.SwiftSearch.SearchHandlers
{
    public static class SearchHandlerFactory
    {
        private static readonly Dictionary<string, Func<string, ISearchHandler>> NameOfSearchHandlers = new Dictionary<string, Func<string, ISearchHandler>>
            (StringComparer.OrdinalIgnoreCase)
        {
            {"", s => new EverythingSearchHandler(s)},   
            {"g", s => new GoodleSearchHandler(s)},   
            {"vk", s => new VkSearchHandler(s)},   
            {"zn", s => new ZaycevNetSearchHandler(s)}   
        };

        public static ISearchHandler Create(string name, string search)
        {
            if(!NameOfSearchHandlers.ContainsKey(name))
                throw new ArgumentException(String.Format("Can't find factory for name \"{0}\"",name),"name");
            Contract.EndContractBlock();

            return NameOfSearchHandlers[name](search);
        }
    }
}