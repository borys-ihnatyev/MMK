using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using MMK.Wpf;
using MMK.Wpf.ViewModel;

namespace MMK.Notify.ViewModels.HashTagFolders
{
    public class FolderViewModel : ViewModel
    {
        private string path;
        private bool isValidPath;

        public FolderViewModel()
        {
            Patterns = new ObservableCollection<FolderPatternViewModel>();
            AddPatternCommand = new Command(AddPatternCommandAction);
            RemovePatternCommand = new Command<FolderPatternViewModel>(RemovePatternCommandAction);
            PickFolderCommand = new Command(PickFolderCommandAction);

            path = "";
        }

        public FolderViewModel(string path) : this()
        {
            Path = path;
        }

        public string Path
        {
            get { return path; }
            set
            {
                if (value == path) return;

                path = value;
                NotifyPropertyChanged();

                IsValidPath = Directory.Exists(path);
            }
        }

        public bool IsValidPath
        {
            get { return isValidPath; }
            private set
            {
                if (value == isValidPath) return;

                isValidPath = value;
                NotifyPropertyChanged();
            }
        }

        public ObservableCollection<FolderPatternViewModel> Patterns { get; set; }

        public ICommand AddPatternCommand { get; private set; }

        private void AddPatternCommandAction()
        {
            Patterns.Add(new FolderPatternViewModel());
        }

        public ICommand RemovePatternCommand { get; private set; }

        private void RemovePatternCommandAction(FolderPatternViewModel patternViewModel)
        {
            Patterns.Remove(patternViewModel);
        }

        public ICommand PickFolderCommand { get; private set; }

        private void PickFolderCommandAction()
        {
            throw new NotImplementedException();
        }
    }
}