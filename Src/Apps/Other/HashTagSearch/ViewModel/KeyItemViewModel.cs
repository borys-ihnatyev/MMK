using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MMK.HashTagSearch.ViewModel
{
    public class KeyItemViewModel
    {
        public KeyItemViewModel(Key key, Color keyColor)
        {
            KeyText = key.ToString(KeyNotation.SharpStripMajorMinor);
            KeyColor = keyColor;
        }
        public string KeyText { get; private set; }
        public Color KeyColor { get; private set; }
    }
}
