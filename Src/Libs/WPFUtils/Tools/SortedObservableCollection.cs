using System.Collections.Generic;
using System.ComponentModel;

namespace System.Collections.ObjectModel
{
    public class SortedObservableCollection<T> : ObservableCollection<T>
    {
        public SortedObservableCollection(IComparer<T> comparer)
        {
            this.comparer = comparer;
        }

        public SortedObservableCollection(IEnumerable<T> collection, IComparer<T> comparer)
            : base(collection)
        {
            this.comparer = comparer;
        }

        public SortedObservableCollection(List<T> list, IComparer<T> comparer)
            : base(list)
        {
            this.comparer = comparer;
        }

        #region functional
        private readonly IComparer<T> comparer;

        private int GetItemIndex(T item)
        {
            if (Count == 0)
                return 0;

            for (int i = Count - 1; i > -1; i--)
            {
                int compareResult = comparer.Compare(item, this[i]);
                if (compareResult == 1 || compareResult == 0)
                    return i + 1;
            }
            return 0;
        }

        private void InsertItem(T item) 
        {
            if (item is INotifyPropertyChanged) 
            {
                var changeableItem = (INotifyPropertyChanged)item;
                changeableItem.PropertyChanged += OnItemChanged;
            }
            base.InsertItem(GetItemIndex(item), item);
        }

        private void OnItemChanged(object sender, PropertyChangedEventArgs e)
        {
            int itemIndex = IndexOf((T)sender);
            UpdateItem(itemIndex);
        }

        private void UpdateItem(int index) 
        {
            var item = this[index];
            RemoveItem(index);
            InsertItem(item);
        }
        #endregion

        protected override void InsertItem(int index, T item)
        {
            InsertItem(item);
        }

        protected override void RemoveItem(int index)
        {
            var item = this[index] as INotifyPropertyChanged;
            if(item != null)
                item.PropertyChanged -= OnItemChanged;
              
            base.RemoveItem(index);
        }

        protected override void SetItem(int index, T item)
        {
            RemoveItem(index);
            InsertItem(item);
        }
    }
}
