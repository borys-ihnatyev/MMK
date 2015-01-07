﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using MMK.HotMark.Model;
using MMK.HotMark.Model.Files;
using MMK.HotMark.ViewModels.PianoKeyBoard;
using MMK.HotMark.Views;
using MMK.Marking;
using MMK.Wpf.ViewModel;

namespace MMK.HotMark.ViewModels
{
    public class HotMarkViewModel : ViewModel
    {
        private string fileItemView;
        private readonly FileHashTagCollection files;
        private HashTagModelChangeNotify hashTagModelChangeNotify;

        private readonly PianoKeyBoardViewModel pianoKeyBoardViewModel;

        private bool isPianoKeyboardLayout;
        private bool canDirectEditHashTags;

        #region Ctors

        public HotMarkViewModel()
        {
            files = new FileHashTagCollection();
            HashTags = new HashTagCollectionViewModel();
            HashTags.CollectionChanged += OnHashTagCollectionViewModelCollectionChanged;
            pianoKeyBoardViewModel = new PianoKeyBoardViewModel();
        }

        public HotMarkViewModel(IEnumerable<string> paths) : this()
        {
            paths.ForEach(files.Add);
        }

        #endregion

        public string FileItemView
        {
            get { return fileItemView; }
            internal set
            {
                if (value == fileItemView) return;

                fileItemView = value;
                NotifyPropertyChanged();
            }
        }

        public HashTagCollectionViewModel HashTags { get; private set; }

        public PianoKeyBoardViewModel PianoKeyBoardViewModel
        {
            get { return pianoKeyBoardViewModel; }
        }

        public PlayerViewModel PlayerViewModel { get; private set; }


        public bool IsPianoKeyboardLayout
        {
            get { return isPianoKeyboardLayout; }
            private set
            {
                if (!(isPianoKeyboardLayout ^ value)) return;
                isPianoKeyboardLayout = value;
                UpdateCanDirectEditHashTags();
                NotifyPropertyChanged();
            }
        }

        public bool CanDirectEditHashTags
        {
            get { return canDirectEditHashTags; }
            private set
            {
                if (value == canDirectEditHashTags)
                    return;
                canDirectEditHashTags = value;
                NotifyPropertyChanged();
            }
        }

        private void UpdateCanDirectEditHashTags()
        {
            CanDirectEditHashTags = !HashTags.IsEmpty && !isPianoKeyboardLayout;
        }

        private void OnHashTagCollectionViewModelCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add || e.Action == NotifyCollectionChangedAction.Remove)
                UpdateCanDirectEditHashTags();
        }

        #region Loading

        protected override void OnLoadData()
        {
            LoadFilePaths();
            LoadHashTagModels();
            pianoKeyBoardViewModel.PropertyChanged += PianoKeyBoardViewModelOnPropertyChanged;
        }

        /// <summary>
        ///     Loads file paths from command line args
        /// </summary>
        /// <returns>true if file paths was in command line</returns>
        private void LoadFilePaths()
        {
            var commandLineArgs = Environment.GetCommandLineArgs();

            if (commandLineArgs.Length <= 1 && files.Count == 0)
                Shutdown();

            for (var i = 1; i < commandLineArgs.Length; i++)
                files.Add(commandLineArgs[i]);

            if (files.Count == 0)
                Shutdown();

            if (files.Count == 1)
            {
                PlayerViewModel = new PlayerViewModel(files.First().Path);
                PlayerViewModel.FileOpened += PlayerViewModelOnFileOpened;
            }
            else
                FileItemView = string.Format("< {0} files >", files.Count);
        }

        private void PlayerViewModelOnFileOpened(object sender, EventArgs eventArgs)
        {
            PlayerViewModel.Position = PlayerViewModel.PositionMax*0.4;
            PlayerViewModel.Volume = 0.3;
            PlayerViewModel.Play();
        }

        private void LoadHashTagModels()
        {
            LoadHashTags();
            hashTagModelChangeNotify = HashTagModelChangeNotify.Create(files);
        }

        private void LoadHashTags()
        {
            foreach (var hashTag in files.ConjointHashTagModel)
                HashTags.Add(new HashTagViewModel(hashTag.TagValue));
        }


        private void PianoKeyBoardViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "RecognizedKey")
                return;

            if (HashTags.IsEmpty)
                HashTags.Add();

            HashTags.Selected.HashTagValue = pianoKeyBoardViewModel.RecognizedKey.ToString();
        }

        #endregion

        #region Commands

        public ICommand ChangeLayoutCommand { get; private set; }

        public void ChangeLayout()
        {
            IsPianoKeyboardLayout = !IsPianoKeyboardLayout;
        }


        public ICommand KeyDownCommand { get; private set; }

        public void KeyDown(KeyEventArgs e)
        {
            if (!IsPianoKeyboardLayout)
                return;

            pianoKeyBoardViewModel.KeyDownCommand.Execute(e);
        }


        public ICommand KeyUpCommand { get; private set; }

        public void KeyUp(KeyEventArgs e)
        {
            if (!IsPianoKeyboardLayout)
                return;

            pianoKeyBoardViewModel.KeyUpCommand.Execute(e);
        }


        public ICommand CloseCommand { get; private set; }

        public void Close()
        {
            if (PlayerViewModel != null)
                PlayerViewModel.Pause();

            hashTagModelChangeNotify.NotifyChange(GetNotEmptyHashTags());
            CloseView();
        }

        private IEnumerable<HashTag> GetNotEmptyHashTags()
        {
            return HashTags
                .Select(item => item.HashTag)
                .Where(hashTag => !hashTag.IsEmpty());
        }


        public ICommand CloseViewCommand { get; private set; }

        public void CloseView()
        {
            var view = Application.Current.Windows
                .OfType<HotMarkMainView>()
                .FirstOrDefault(v => v.DataContext == this);

            if (view != null)
                view.Close();
        }

        #endregion

        private void Shutdown()
        {
#if DEBUG
            throw new Exception("Supposed any file path as cl argument");
#else
            CloseView();
#endif
        }
    }
}