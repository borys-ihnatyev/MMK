using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MMK.Search;
using System.IO;

namespace MMK.HashTagSearch.Model
{
    public class SearchContentModel
    {
        private int count;

        private string searchString;
        private string modifyedSearchString;

        public string SearchString
        {
            get
            {
                return searchString;
            }
            set
            {
                value = value.Trim();
                if (value != searchString)
                {
                    if (value == string.Empty)
                    {
                        searchString = string.Empty;
                        modifyedSearchString = string.Empty;
                    }
                    else
                    {
                        searchString = value;
                        modifyedSearchString = string.Format("*{0}*.mp3", searchString);
                    }
                }
            }
        }

        public string[] GetFiles()
        {
            string[] result = new string[0];
            if (modifyedSearchString != string.Empty)
                try
                {
                    result = Directory.GetFiles(@"D:\music\main\", modifyedSearchString, SearchOption.AllDirectories);
                }
                catch (ArgumentException) { }
            count = result.Length;

            return result;
        }

        public int Count
        {
            get
            {
                return count;
            }
        }
    }
}
