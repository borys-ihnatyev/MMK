using MMK.SwiftSearch.Model;

namespace MMK.SwiftSearch.SearchHandlers
{
    sealed public class GoodleSearchHandler : WebSearchHandler
    {
        public const string Identifyer = "@g";

        public GoodleSearchHandler(string searchModel) : base(searchModel)
        {

        }

        public override string IdentifyerTag
        {
            get { return Identifyer; }
        }

        protected override string UrlFormat
        {
            get { return "http://www.google.com/search?q={0}"; }
        }
    }
}
