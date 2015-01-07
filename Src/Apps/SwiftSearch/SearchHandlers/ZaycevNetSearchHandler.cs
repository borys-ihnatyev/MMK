using MMK.SwiftSearch.Model;

namespace MMK.SwiftSearch.SearchHandlers
{
    sealed public class ZaycevNetSearchHandler : WebSearchHandler
    {
        public ZaycevNetSearchHandler(string searchModel) : base(searchModel)
        {
        }

        protected override string UrlFormat
        {
            get { return "http://go.mail.ru/search_site?fr=main&p=1&aux=Kd7dJd&q={0}"; }
        }
    }
}
