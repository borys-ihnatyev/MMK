using System.Windows.Media;
using MMK.Marking;
using MMK.Wpf;
using MMK.Wpf.ViewModel;

namespace MMK.HotMark.ViewModel.Main
{
    public partial class HashTagViewModel : ObservableObject
    {
        private HashTag hashTag;
        private string hashTagValue;
        private bool isSelected;
        private SolidColorBrush itemBackground;

        public HashTagViewModel(string hashTagValue)
        {
            HashTagValue = hashTagValue;
        }

        public HashTagViewModel()
            : this(string.Empty)
        {
        }

        public bool IsSelected
        {
            get { return isSelected; }
            internal set
            {
                if (isSelected == value) return;

                isSelected = value;
                NotifyPropertyChanged();
            }
        }

        public string HashTagValue
        {
            get { return hashTagValue; }
            set
            {
                value = value.Replace(" ", "").ToLower();
                if (value == hashTagValue) return;

                hashTagValue = value;

                var hashTagEntry = HashTag.Parser.First(HashTag.Hash + hashTagValue);
                    
                HashTag = hashTagEntry == null ? new HashTag() : hashTagEntry.HashTag ;

                NotifyPropertyChanged();
            }
        }

        public SolidColorBrush ItemBackground
        {
            get { return itemBackground; }
            set
            {
                if (Equals(value, itemBackground)) return;

                itemBackground = value;
                NotifyPropertyChanged();
            }
        }

        public HashTag HashTag
        {
            get { return hashTag; }
            private set
            {
                if (Equals(value, hashTag)) return;

                hashTag = value;
                if (hashTag is KeyHashTag)
                    ItemBackground = KeyColorConverter.Convert((hashTag as KeyHashTag).Key);
                else
                    ItemBackground = Brushes.White;

                NotifyPropertyChanged();
            }
        }
    }
}