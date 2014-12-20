using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using MMK.Marking;
using MMK.Marking.Representation;
using MMK.Wpf;
using MMK.Wpf.ViewModel;

namespace MMK.MagicPlaylist.ViewModels
{
    public class MainViewModel : ViewModel
    {
        private string patternString;
        private HashTagModel pattern;

        public MainViewModel()
        {
            patternString = string.Empty;
            pattern = new HashTagModel();

            SourceDirs = new DirectoryCollection();

            SanitizeSourceDirsCommand = new Command(SourceDirs.Sanitize);
            AddSourceDirsFromDropCommand = new Command<DragEventArgs>(AddSourceDirsFromDrop);
            RemoveSourceDirCommand = new Command<string>(dir => SourceDirs.Remove(dir));
        }


        public string PatternString
        {
            get { return patternString; }
            set
            {
                if (value == null)
                    value = string.Empty;

                patternString = value;
                Pattern = HashTagModel.Parser.All(value);
                NotifyPropertyChanged();
            }
        }

        public HashTagModel Pattern
        {
            get { return new HashTagModel(pattern); }
            private set
            {
                if (value == null)
                    value = new HashTagModel();

                if (value.SetEquals(pattern))
                    return;

                pattern = value;

                NotifyPropertyChanged();
            }
        }

        public DirectoryCollection SourceDirs { get; set; }


        protected override string OnValidation(string columnName)
        {
            if (columnName == "Pattern" || columnName == "patternString")
                if (Pattern.Any(h => h is KeyHashTag))
                    return "Can't hold key";

            return base.OnValidation(columnName);
        }


        public ICommand SanitizeSourceDirsCommand { get; private set; }
        public ICommand AddSourceDirsFromDropCommand { get; private set; }
        public ICommand RemoveSourceDirCommand { get; private set; }
        public ICommand GenerateCommand { get; private set; }

        private void AddSourceDirsFromDrop(DragEventArgs e)
        {
            var items = e.Data.GetData(DataFormats.FileDrop) as string[];
            if (items != null)
                items.ForEach(SourceDirs.Add);
        }
    }
}