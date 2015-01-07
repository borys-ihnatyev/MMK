using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using MMK.Wpf;

namespace MMK.HotMark.ViewModels
{
    public class HashTagCollectionViewModel : SortedObservableCollection<HashTagViewModel>
    {
        private HashTagViewModel selected;

        public HashTagCollectionViewModel() : base(new HashTagViewModel.Comparer())
        {
            AddNewCommand = new Command(Add);
            RemoveCommand = new Command<HashTagViewModel>(vm => Remove(vm));
            SelectCommand = new Command<HashTagViewModel>(vm => Selected = vm);
        }

        public HashTagViewModel Selected
        {
            get { return selected; }
            set
            {
                if (value == selected) return;

                if (selected != null)
                    selected.IsSelected = false;

                selected = value;

                if (selected != null)
                    selected.IsSelected = true;

                OnPropertyChanged(new PropertyChangedEventArgs("Selected"));
            }
        }

        public bool IsEmpty
        {
            get { return Count == 0; }
        }

        #region Overrides

        protected override void InsertItem(int index, HashTagViewModel item)
        {
            if (item == null || item.IsEmpty)
                InsetEmptyItem(index, item);
            else
                InsertNotEmptyItem(index, item);
        }

        private void InsetEmptyItem(int index, HashTagViewModel item)
        {
            var emptyHashTag = GetEmptyHashTag();
                
            if (emptyHashTag == null)
            {
                emptyHashTag = item;
                base.InsertItem(index, emptyHashTag);
            }

            Selected = emptyHashTag;
        }

        private void InsertNotEmptyItem(int index, HashTagViewModel item)
        {
            base.InsertItem(index, item);
            if (Selected == null)
                Selected = item;
        }

        protected override void RemoveItem(int index)
        {
            base.RemoveItem(index);

            if (IsEmpty)
                Selected = null;
            else
                SelectNext();
        }

        protected override void ClearItems()
        {
            base.ClearItems();

            Selected = null;
        }

        protected override void OnItemPropertyChanged(HashTagViewModel item)
        {
            if (item.IsEmpty)
            {
                var emptyHashTag = GetEmptyHashTag();

                if (emptyHashTag == null || emptyHashTag == item)
                    base.OnItemPropertyChanged(item);
                else
                {
                    Remove(emptyHashTag);
                    base.OnItemPropertyChanged(item);                    
                    Selected = item;
                }    
            }
            else
                base.OnItemPropertyChanged(item);                                    
        }

        #endregion

        #region Commands

        public ICommand AddNewCommand { get; private set; }

        public void Add()
        {
            Add(new HashTagViewModel());
        }

        private HashTagViewModel GetEmptyHashTag()
        {
            return Items.LastOrDefault(vm => vm.IsEmpty);
        }

        public ICommand RemoveCommand { get; private set; }

        public ICommand SelectCommand { get; private set; }


        public ICommand SelectNextCommand { get; private set; }

        public void SelectNext()
        {
            if (IsEmpty) return;

            var selectedHashTagItemIndex = IndexOf(Selected);

            if (selectedHashTagItemIndex == Count - 1)
                selectedHashTagItemIndex = 0;
            else
                ++selectedHashTagItemIndex;

            Selected = Items[selectedHashTagItemIndex];
        }


        public ICommand SelectPreviousCommand { get; private set; }

        public void SelectPrevious()
        {
            if (IsEmpty) return;

            var selectedHashTagItemIndex = IndexOf(Selected);

            if (selectedHashTagItemIndex == 0)
                selectedHashTagItemIndex = Count - 1;
            else
                --selectedHashTagItemIndex;

            Selected = Items[selectedHashTagItemIndex];
        }

        #endregion
    }
}