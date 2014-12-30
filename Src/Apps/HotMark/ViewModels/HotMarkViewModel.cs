using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Odbc;
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

        private HashTagViewModel selectedHashTag;
        private readonly PianoKeyBoardViewModel pianoKeyBoardViewModel;

        private bool isPianoKeyboardLayout;
        private bool canDirectEditHashTags;

        #region Ctors

        public HotMarkViewModel()
        {
            files = new FileHashTagCollection();
            HashTags = new SortedObservableCollection<HashTagViewModel>(new HashTagViewModel.Comparer());

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

        public SortedObservableCollection<HashTagViewModel> HashTags { get; set; }

        public HashTagViewModel SelectedHashTag
        {
            get { return selectedHashTag; }
            set
            {
                if (value == selectedHashTag) return;

                if (selectedHashTag != null)
                    selectedHashTag.IsSelected = false;

                selectedHashTag = value;

                if (selectedHashTag != null)
                    selectedHashTag.IsSelected = true;

                NotifyPropertyChanged();
            }
        }

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

        public bool HasHashTags
        {
            get { return HashTags.Count > 0; }
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
            CanDirectEditHashTags = HasHashTags && !isPianoKeyboardLayout;
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

        /// <summary>
        ///     Intersect all files hash tag models to HashTags member
        /// </summary>
        private void LoadHashTagModels()
        {
            LoadHashTags();

            hashTagModelChangeNotify = HashTagModelChangeNotify.Create(files);

            TrySelectFirstHashTag();
        }

        private void LoadHashTags()
        {
            foreach (var hashTag in files.ConjointHashTagModel)
                HashTags.Add(new HashTagViewModel(hashTag.TagValue));
        }

        private void TrySelectFirstHashTag()
        {
            if (!HasHashTags) return;

            SelectedHashTag = HashTags[0];

            UpdateCanDirectEditHashTags();
        }


        private void PianoKeyBoardViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "RecognizedKey")
                return;
            if (!HasHashTags)
                AddHashTag();
            selectedHashTag.HashTagValue = pianoKeyBoardViewModel.RecognizedKey.ToString();
        }

        #endregion

        #region Commands

        public ICommand ChangeLayoutCommand { get; private set; }

        public void ChangeLayout()
        {
            IsPianoKeyboardLayout = !IsPianoKeyboardLayout;
        }


        public ICommand SelectHashTagCommand { get; private set; }

        public void SelectHashTag(HashTagViewModel item)
        {
            SelectedHashTag = item;
        }


        public ICommand AddHashTagCommand { get; private set; }

        public void AddHashTag()
        {
            var emptyHashTag = HashTags.FirstOrDefault(vm => vm.HashTag.IsEmpty());

            if (emptyHashTag == null)
            {
                SelectedHashTag = new HashTagViewModel();
                HashTags.Add(SelectedHashTag);
                UpdateCanDirectEditHashTags();
            }
            else
            {
                SelectedHashTag = emptyHashTag;
            }
        }


        public ICommand RemoveHashTagCommand { get; private set; }

        public void RemoveHashTag(HashTagViewModel hashTag)
        {
            HashTags.Remove(hashTag);

            if (SelectedHashTag == hashTag)
                SelectNextOrResetSelectedHashTag();

            UpdateCanDirectEditHashTags();
        }

        private void SelectNextOrResetSelectedHashTag()
        {
            if (HasHashTags)
                SelectNextHashTag();
            else
                SelectedHashTag = null;
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

        public ICommand SelectNextHashTagCommand { get; private set; }

        public void SelectNextHashTag()
        {
            if (!HasHashTags) return;

            var selectedHashTagItemIndex = HashTags.IndexOf(SelectedHashTag);

            if (selectedHashTagItemIndex == HashTags.Count - 1)
                selectedHashTagItemIndex = 0;
            else
                ++selectedHashTagItemIndex;

            SelectedHashTag = HashTags[selectedHashTagItemIndex];
        }

        public ICommand SelectPreviousHashTagCommand { get; private set; }

        public void SelectPreviousHashTag()
        {
            if (!HasHashTags) return;

            var selectedHashTagItemIndex = HashTags.IndexOf(SelectedHashTag);

            if (selectedHashTagItemIndex == 0)
                selectedHashTagItemIndex = HashTags.Count - 1;
            else
                --selectedHashTagItemIndex;

            SelectedHashTag = HashTags[selectedHashTagItemIndex];
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