using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MMK.HashTagSearch.ViewModel
{
    public class ParalelKeysViewModel
    {
        public ParalelKeysViewModel(Key major, Key minor)
        {
            Major = major;
            Minor = minor;
        }

        public Key Major { get; private set; }
        public Key Minor { get; private set; }
    }
}
