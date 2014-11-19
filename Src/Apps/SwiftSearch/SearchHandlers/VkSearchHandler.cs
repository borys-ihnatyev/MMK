using MMK.SwiftSearch.Model;

namespace MMK.SwiftSearch.SearchHandlers
{
    sealed public class VkSearchHandler : WebSearchHandler
    {
        public const string Identifyer = "@vk";

        public VkSearchHandler(string searchModel) : base(searchModel)
        {
        }

        public override string IdentifyerTag
        {
            get { return Identifyer; }
        }

        protected override string UrlFormat
        {
            get { return "http://vk.com/search?c[q]={0}&c[section]=audio"; }
        }
    }
}
