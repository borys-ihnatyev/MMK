using MMK.Marking.Representation;

namespace MMK.Notify.ViewModel.HashTagFolders
{
    public class FolderPatternViewModel : Wpf.ViewModel.ViewModel
    {
        private string patternString;
        private int priority;

        private bool isValidPattern;

        public FolderPatternViewModel()
        {
            patternString = "";
            priority = 0;
        }

        public bool IsValidPattern
        {
            get { return isValidPattern; }
            set
            {
                if(value == isValidPattern) return;
                
                isValidPattern = value;
                NotifyPropertyChanged();
            }
        }

        public string PatternString
        {
            get { return patternString; }
            set
            {
                if(value == patternString) return;

                patternString = value;
                NotifyPropertyChanged();

                Pattern = HashTagModel.Parser.All(patternString);
                IsValidPattern = Pattern.Count > 0;
            }
        }

        public HashTagModel Pattern
        {
            get; private set;
        }

        public int Priority
        {
            get { return priority; }
            set
            {
                if(value == priority) 
                    return;
                priority = value;
                NotifyPropertyChanged();
            }
        }
    }
}