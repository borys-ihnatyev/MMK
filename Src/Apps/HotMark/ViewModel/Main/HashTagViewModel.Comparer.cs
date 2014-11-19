using System.Collections.Generic;
using MMK.Marking;

namespace MMK.HotMark.ViewModel.Main
{
    public partial class HashTagViewModel
    {
        public class Comparer : HashTag.Comparer, IComparer<HashTagViewModel>
        {
            public int Compare(HashTagViewModel x, HashTagViewModel y)
            {
                if (x == y) return Equal;
                if (x == null) return Higher;
                if (y == null) return Lower;

                return Compare(x.HashTag, y.HashTag);
            }
        }
    }
}
