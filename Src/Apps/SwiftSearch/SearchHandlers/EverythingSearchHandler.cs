using System.Diagnostics;
using System.IO;
using MMK.SwiftSearch.Model;

namespace MMK.SwiftSearch.SearchHandlers
{
    sealed class EverythingSearchHandler : ISearchHandler
    {
        private const string EverythingExePathX64 = @"C:\Program Files\Everything\Everything.exe";
        private const string EverythingExePathX86 = @"C:\Program Files(x86)\Everything\Everything.exe";
        private const string SearchUrlFormat = "-s \"{0}\" -filter Audio -sort Path";

        public EverythingSearchHandler(string searchModel)
        {
            SearchModel = searchModel.Trim();
        }

        public string SearchModel { get; private set; }

        public void Search()
        {
            if (File.Exists(EverythingExePathX64)) 
                Process.Start(EverythingExePathX64, GetClArgs(SearchModel));
            else if (File.Exists(EverythingExePathX86))
                Process.Start(EverythingExePathX86, GetClArgs(SearchModel));
        }

        private static string GetClArgs(string search)
        {
            return string.Format(SearchUrlFormat, search);
        }

    }
}
