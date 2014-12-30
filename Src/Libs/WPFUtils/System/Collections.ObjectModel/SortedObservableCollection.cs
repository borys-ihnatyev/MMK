using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;

namespace System.Collections.ObjectModel
{
    public class SortedObservableCollection<T> : ObservableCollection<T> where T : INotifyPropertyChanged
    {
        private readonly IComparer<T> comparer;

        #region ctors

        public SortedObservableCollection(IComparer<T> comparer)
        {
            if (comparer == null)
                throw new ArgumentNullException("comparer");
            Contract.EndContractBlock();

            this.comparer = comparer;
        }

        public SortedObservableCollection(IEnumerable<T> collection, IComparer<T> comparer)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");
            if (comparer == null)
                throw new ArgumentNullException("comparer");
            Contract.EndContractBlock();

            this.comparer = comparer;

            foreach (var item in collection)
                Add(item);
        }

        #endregion

        protected override void InsertItem(int index, T item)
        {
            InsertItem(item);
        }

        private void InsertItem(T item)
        {
            item.PropertyChanged += OnItemChanged;
            base.InsertItem(GetItemIndex(item), item);
        }

        private int GetItemIndex(T item)
        {
            if (Count == 0)
                return 0;

            for (var i = Count - 1; i > -1; i--)
            {
                var compareResult = comparer.Compare(item, this[i]);
                if (compareResult == 1 || compareResult == 0)
                    return i + 1;
            }
            return 0;
        }

        protected override void RemoveItem(int index)
        {
            this[index].PropertyChanged -= OnItemChanged;
            base.RemoveItem(index);
        }

        private void OnItemChanged(object sender, PropertyChangedEventArgs e)
        {
            var itemIndex = IndexOf((T) sender);
            UpdateItem(itemIndex);
        }

        protected virtual void UpdateItem(int index)
        {
            var item = this[index];
            RemoveItem(index);
            InsertItem(item);
        }

        protected override void SetItem(int index, T item)
        {
            RemoveItem(index);
            InsertItem(item);
        }
    }
}