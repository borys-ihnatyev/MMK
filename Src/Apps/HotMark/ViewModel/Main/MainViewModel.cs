using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using MMK.HotMark.Model;
using MMK.HotMark.Model.Files;
using MMK.HotMark.View;
using MMK.HotMark.ViewModel.PianoKeyBoard;
using MMK.Marking;
using MMK.Wpf;

namespace MMK.HotMark.ViewModel.Main
{
    public class MainViewModel : Wpf.ViewModel.ViewModel
    {
        private readonly FileHashTagCollection files;
        private readonly MainView window = Application.Current.Windows.OfType<MainView>().First();
        private string fileItemView;

        private bool hasHashTags;
        private HashTagModelChangeNotify hashTagModelChangeNotify;

        private HashTagViewModel selectedHashTag;

        private readonly PianoKeyBoardViewModel pianoKeyBoardViewModel;

        private bool isPianoKeyboardLayout;
        private bool canDirectEditHashTags;


        public MainViewModel()
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

        public MainViewModel(IEnumerable<string> paths) : this()
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
                CanDirectEditHashTags = hasHashTags && !isPianoKeyboardLayout;

                NotifyPropertyChanged();
            }
        }

        public bool HasHashTags
        {
            get { return hasHashTags; }
            private set
            {
                if (value == hasHashTags) return;

                hasHashTags = value;
                CanDirectEditHashTags = hasHashTags && !isPianoKeyboardLayout;
                NotifyPropertyChanged();
            }
        }

        public bool CanDirectEditHashTags
        {
            get { return canDirectEditHashTags; }
            private set
            {
                if(!(value ^ canDirectEditHashTags)) return;
                
                canDirectEditHashTags = value;
                NotifyPropertyChanged();
            }
        }

        #region Loading

        protected override void OnLoadData()
        {
            LoadFilePaths();
            LoadHashTagModels();
            pianoKeyBoardViewModel.PropertyChanged += PianoKeyBoardViewModelOnPropertyChanged;
        }

        private void PianoKeyBoardViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if(propertyChangedEventArgs.PropertyName != "RecognizedKey")
                return;
            if(!HasHashTags)
                AddHashTag();
            selectedHashTag.HashTagValue = pianoKeyBoardViewModel.RecognizedKey.ToString();
        }

        /// <summary>
        ///     Loads file paths from command line args
        /// </summary>
        /// <returns>true if file paths was in command line</returns>
        private void LoadFilePaths()
        {
            string[] commandLineArgs = Environment.GetCommandLineArgs();

            if (commandLineArgs.Length <= 1 && files.Count == 0)
                Shutdown();

            for (int i = 1; i < commandLineArgs.Length; i++)
                files.Add(commandLineArgs[i]);

            if (files.Count == 0)
                Shutdown();

            FileItemView = (files.Count == 1)
                ? files.First().ClearName
                : string.Format("< {0} files >", files.Count);
        }

        private void Shutdown()
        {
#if DEBUG
            throw new Exception("Supposed any file path as cl argument");
#else
            window.Close();
#endif
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

        private void LoadHashTags()
        {
            foreach (HashTag hashTag in files.ConjointHashTagModel)
                HashTags.Add(new HashTagViewModel(hashTag.TagValue));
        }

        private void SetHashTagModelChangeNotify()
        {
            if (HashTags.Count == GetFileItemMaxHashTagCount())
                hashTagModelChangeNotify = new RewriteHashTagModel(files.GetPaths(), files.ConjointHashTagModel);
            else
                hashTagModelChangeNotify = new ChangeHashTagModel(files.GetPaths(), files.ConjointHashTagModel);
        }

        private void TrySelectFirstHashTag()
        {
            if (HashTags.Count <= 0) return;

            SelectedHashTag = HashTags[0];
            HasHashTags = true;
            SelectNextOrResetSelectedHashTag();
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
            HasHashTags = true;
        }


        public ICommand RemoveHashTagCommand { get; private set; }

        private void RemoveHashTag(HashTagViewModel item)
        {
            if (SelectedHashTag == item)
                SelectNextOrResetSelectedHashTag();

            HashTags.Remove(item);
        }

        
        public ICommand KeyDownCommand { get; private set; }

        private void KeyDown(KeyEventArgs e)
        {
            if(e.IsRepeat)
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
                else if(e.Key == System.Windows.Input.Key.Enter)
                    Close();
            
            if (e.Key == System.Windows.Input.Key.Escape)
                window.Close();
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
            if(!IsPianoKeyboardLayout)
                return;

            pianoKeyBoardViewModel.KeyUpCommand.Execute(e);
        }


        public ICommand CloseCommand { get; private set; }

        private void Close()
        {
            hashTagModelChangeNotify.NotifyChange(GetNotEmptyHashTags());
            window.Close();
        }


        private IEnumerable<HashTag> GetNotEmptyHashTags()
        {
            return HashTags
                .Select(item => item.HashTag)
                .Where(hashTag => !hashTag.IsEmpty());
        }

        #endregion

        private int GetFileItemMaxHashTagCount()
        {
            return files.Count == 0
                ? 0
                : files.Select(fileItem => fileItem.HashTagModel.Count).Max();
        }

        private void SelectNextOrResetSelectedHashTag()
        {
            if (HashTags.Count > 1)
                SelectNextHashTag();
            else
                ResetSelectedHashTag();
        }

        private void SelectNextHashTag()
        {
            if (!HasHashTags) return;

            int selectedHashTagItemIndex = HashTags.IndexOf(SelectedHashTag);

            if (selectedHashTagItemIndex == HashTags.Count - 1)
                selectedHashTagItemIndex = 0;
            else
                ++selectedHashTagItemIndex;

            SelectedHashTag = HashTags[selectedHashTagItemIndex];
        }

        private void SelectPreviousHashTag()
        {
            if (!HasHashTags) return;

            int selectedHashTagItemIndex = HashTags.IndexOf(SelectedHashTag);

            if (selectedHashTagItemIndex == 0)
                selectedHashTagItemIndex = HashTags.Count - 1;
            else
                --selectedHashTagItemIndex;

            SelectedHashTag = HashTags[selectedHashTagItemIndex];
        }

        private void ResetSelectedHashTag()
        {
            SelectedHashTag = null;
            HasHashTags = false;
        }
    }
}