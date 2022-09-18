using MMK.SwiftSearch.Model;

namespace MMK.SwiftSearch.SearchHandlers
{
    sealed class VkSearchHandler : WebSearchHandler
    {
        public VkSearchHandler(string searchModel) : base(searchModel)
        {
        }

        protected override string UrlFormat
        {
            get { return "http://vk.com/search?c[q]={0}&c[section]=audio"; }
        }
    }
}
