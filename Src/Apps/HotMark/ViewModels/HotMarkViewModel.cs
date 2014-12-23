using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using MMK.HotMark.Model;
using MMK.HotMark.Model.Files;
using MMK.HotMark.ViewModels.PianoKeyBoard;
using MMK.HotMark.Views;
using MMK.Marking;
using MMK.Wpf;

namespace MMK.HotMark.ViewModels
{
    public class HotMarkViewModel : Wpf.ViewModel.ViewModel
    {
        private readonly FileHashTagCollection files;
        private HashTagModelChangeModel hashTagModelChangeModel;

        private string fileItemView;

        private HashTagViewModel selectedHashTag;

        private readonly PianoKeyBoardViewModel pianoKeyBoardViewModel;

        private bool isPianoKeyboardLayout;
        private bool canDirectEditHashTags;

        public HotMarkViewModel()
        {
            files = new FileHashTagCollection();
            HashTags = new SortedObservableCollection<HashTagViewModel>(new HashTagViewModel.Comparer());

            pianoKeyBoardViewModel = new PianoKeyBoardViewModel();

            SelectHashTagCommand = new Command<HashTagViewModel>(SelectHashTag);
            CloseCommand = new Command(Close);

            AddHashTagCommand = new Command(AddHashTag);
            RemoveHashTagCommand = new Command<HashTagViewModel>(RemoveHashTag);

            KeyDownCommand = new Command<KeyEventArgs>(KeyDown);
            KeyUpCommand = new Command<KeyEventArgs>(KeyUp);
        }

        public HotMarkViewModel(IEnumerable<string> paths) : this()
        {
            paths.ForEach(files.Add);
        }

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

            FileItemView = (files.Count == 1)
                ? files.First().ClearName
                : string.Format("< {0} files >", files.Count);
        }

        /// <summary>
        ///     Intersect all files hash tag models to HashTags member
        /// </summary>
        private void LoadHashTagModels()
        {
            LoadHashTags();
            SetHashTagModelChangeNotify();
            TrySelectFirstHashTag();
        }

        private void Shutdown()
        {
#if DEBUG
            throw new Exception("Supposed any file path as cl argument");
#else
            CloseView();
#endif
        }

        private void LoadHashTags()
        {
            foreach (var hashTag in files.ConjointHashTagModel)
                HashTags.Add(new HashTagViewModel(hashTag.TagValue));
        }

        private void SetHashTagModelChangeNotify()
        {
            if (CalcFileItemMaxHashTagCount() == HashTags.Count)
                hashTagModelChangeModel = new RewriteHashTagModel(files.GetPaths(), files.ConjointHashTagModel);
            else
                hashTagModelChangeModel = new ChangeHashTagModel(files.GetPaths(), files.ConjointHashTagModel);
        }

        private int CalcFileItemMaxHashTagCount()
        {
            return files.Count == 0
                ? 0
                : files.Select(fileItem => fileItem.HashTagModel.Count).Max();
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

        public ICommand SelectHashTagCommand { get; private set; }

        private void SelectHashTag(HashTagViewModel item)
        {
            SelectedHashTag = item;
        }


        public ICommand AddHashTagCommand { get; private set; }

        private void AddHashTag()
        {
            SelectedHashTag = new HashTagViewModel();
            HashTags.Add(SelectedHashTag);
            UpdateCanDirectEditHashTags();
        }


        public ICommand RemoveHashTagCommand { get; private set; }

        private void RemoveHashTag(HashTagViewModel hashTag)
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

        private void KeyDown(KeyEventArgs e)
        {
            if (e.IsRepeat)
                return;

            MergedLayoutKeyDown(e);

            if (IsPianoKeyboardLayout)
                pianoKeyBoardViewModel.KeyDownCommand.Execute(e);
            else
                StandartKeyboardLayoutKeyDown(e);
        }

        private void MergedLayoutKeyDown(KeyEventArgs e)
        {
            if (e.KeyboardDevice.Modifiers == ModifierKeys.Control)
                if (e.Key == System.Windows.Input.Key.P)
                    IsPianoKeyboardLayout = !IsPianoKeyboardLayout;
                else if (e.Key == System.Windows.Input.Key.Enter)
                    Close();

            if (e.Key == System.Windows.Input.Key.Escape)
                CloseView();
        }

        private void StandartKeyboardLayoutKeyDown(KeyEventArgs e)
        {
            if (e.KeyboardDevice.Modifiers == ModifierKeys.Control)
                if (e.Key == System.Windows.Input.Key.N)
                    AddHashTag();

            if (e.Key != System.Windows.Input.Key.Tab) return;

            if (e.KeyboardDevice.Modifiers == ModifierKeys.Shift)
                SelectPreviousHashTag();

            if (e.KeyboardDevice.Modifiers == ModifierKeys.None)
                SelectNextHashTag();
        }


        public ICommand KeyUpCommand { get; private set; }

        private void KeyUp(KeyEventArgs e)
        {
            if (!IsPianoKeyboardLayout)
                return;

            pianoKeyBoardViewModel.KeyUpCommand.Execute(e);
        }


        public ICommand CloseCommand { get; private set; }

        private void Close()
        {
            hashTagModelChangeModel.NotifyChange(GetNotEmptyHashTags());
            CloseView();
        }

        private void CloseView()
        {
            var view = Application.Current.Windows
                .OfType<HotMarkMainView>()
                .FirstOrDefault(v => v.DataContext == this);

            if(view != null)
                view.Close();
        }

        private IEnumerable<HashTag> GetNotEmptyHashTags()
        {
            return HashTags
                .Select(item => item.HashTag)
                .Where(hashTag => !hashTag.IsEmpty());
        }

        #endregion

        private void SelectNextHashTag()
        {
            if (!HasHashTags) return;

            var selectedHashTagItemIndex = HashTags.IndexOf(SelectedHashTag);

            if (selectedHashTagItemIndex == HashTags.Count - 1)
                selectedHashTagItemIndex = 0;
            else
                ++selectedHashTagItemIndex;

            SelectedHashTag = HashTags[selectedHashTagItemIndex];
        }

        private void SelectPreviousHashTag()
        {
            if (!HasHashTags) return;

            var selectedHashTagItemIndex = HashTags.IndexOf(SelectedHashTag);

            if (selectedHashTagItemIndex == 0)
                selectedHashTagItemIndex = HashTags.Count - 1;
            else
                --selectedHashTagItemIndex;

            SelectedHashTag = HashTags[selectedHashTagItemIndex];
        }
    }
}