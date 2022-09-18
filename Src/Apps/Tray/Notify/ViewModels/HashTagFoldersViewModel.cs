﻿using System.Collections.ObjectModel;
using System.Windows.Input;
using MMK.ApplicationServiceModel;
using MMK.Notify.ViewModels.HashTagFolders;
using MMK.Presentation.Tools;
using MMK.Presentation.ViewModel;
using MMK.Processing.AutoFolder;

namespace MMK.Notify.ViewModels
{
    public class HashTagFoldersViewModel : ViewModel
    {
        private bool isApplyFailed;

        public HashTagFoldersViewModel()
        {
            AddFolderCommand = new Command(AddFolderCommandAction);
            RemoveFolderCommand = new Command<FolderViewModel>(RemoveFolderCommandAction);
            ApplyCommand = new Command(ApplyCommandAction);

            Folders = new ObservableCollection<FolderViewModel>();
        }

        public ObservableCollection<FolderViewModel> Folders { get; set; }

        public bool IsApplyFailed
        {
            get { return isApplyFailed; }
            private set
            {
                if (value == isApplyFailed) return;
                isApplyFailed = value;
                NotifyPropertyChanged();
            }
        }

        protected override void OnLoadData()
        {
            LoadFolderCollection();
        }

        private void LoadFolderCollection()
        {
            //add folders
            foreach (var path in IoC.Get<HashTagFolderCollection>().Folders)
            {
                var folderViewModel = new FolderViewModel(path);
                foreach (var pattern in IoC.Get<HashTagFolderCollection>().GetPatternsForFolder(path))
                {
                    var folderPatternViewModel = new FolderPatternViewModel
                    {
                        PatternString = pattern.Model.ToString(),
                        Priority = pattern.Priority
                    };
                    folderViewModel.Patterns.Add(folderPatternViewModel);
                }
                Folders.Add(folderViewModel);
            }
        }

        public ICommand AddFolderCommand { get; private set; }

        private void AddFolderCommandAction()
        {
            Folders.Add(new FolderViewModel());
        }

        public ICommand RemoveFolderCommand { get; private set; }

        private void RemoveFolderCommandAction(FolderViewModel folderViewModel)
        {
            Folders.Remove(folderViewModel);
        }

        public ICommand ApplyCommand { get; private set; }

        private void ApplyCommandAction()
        {
            var hashTagFolders = new HashTagFolderCollection();

            foreach (var folderViewModel in Folders)
            {
                if (!folderViewModel.IsValidPath)
                {
                    IsApplyFailed = true;
                    return;
                }

                foreach (var patternViewModel in folderViewModel.Patterns)
                {
                    if (!patternViewModel.IsValidPattern)
                    {
                        IsApplyFailed = true;
                        return;
                    }

                    try
                    {
                        hashTagFolders.Add(folderViewModel.Path, patternViewModel.Pattern);
                    }
                    catch (HashTagFolderCollection.PatternAlreadyExistsException)
                    {
                        IsApplyFailed = true;
                        return;
                    }
                }
            }

            IsApplyFailed = false;
            //App.Current.FolderCollection = hashTagFolders;
        }
    }
}