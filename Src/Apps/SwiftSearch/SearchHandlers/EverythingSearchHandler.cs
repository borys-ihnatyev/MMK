using System.Diagnostics;
using System.IO;
using MMK.SwiftSearch.Model;

namespace MMK.SwiftSearch.SearchHandlers
{
    sealed public class EverythingSearchHandler : SearchHandler
    {
        public const string Idenfyer = "@es";

        private const string EverythingExePathX64 = @"C:\Program Files\Everything\Everything.exe";
        private const string EverythingExePathX86 = @"C:\Program Files(x86)\Everything\Everything.exe";
        private const string SearchUrlFormat = "-s \"{0}\" -filter Audio -sort Path";

        public EverythingSearchHandler(string searchModel) : base(searchModel)
        {

        }

        public override string IdentifyerTag
        {
            get { return Idenfyer; }
        }


        protected override void OnSearch(string pureSearch)
        {
            if (File.Exists(EverythingExePathX64)) 
                Process.Start(EverythingExePathX64, GetClArgs(pureSearch));
            else if (File.Exists(EverythingExePathX86))
                Process.Start(EverythingExePathX86, GetClArgs(pureSearch));
        }

        private static string GetClArgs(string search)
        {
            return string.Format(SearchUrlFormat, search);
        }
    }
}
