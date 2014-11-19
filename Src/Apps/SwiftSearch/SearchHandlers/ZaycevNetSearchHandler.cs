using MMK.SwiftSearch.Model;

namespace MMK.SwiftSearch.SearchHandlers
{
    sealed public class ZaycevNetSearchHandler : WebSearchHandler
    {
        public const string Identifyer = "@zn";

        public ZaycevNetSearchHandler(string searchModel) : base(searchModel)
        {
        }

        public override string IdentifyerTag
        {
            get { return Identifyer; }
        }

        protected override string UrlFormat
        {
            get { return "http://go.mail.ru/search_site?fr=main&p=1&aux=Kd7dJd&q={0}"; }
        }
    }
}
