namespace MMK.SwiftSearch.Model
{
    public interface ISearchHandler
    {
        string SearchModel
        {
            get;
        }
        void Search();
    }
}
