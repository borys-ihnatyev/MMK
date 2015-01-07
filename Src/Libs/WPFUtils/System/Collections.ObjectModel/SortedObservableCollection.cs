using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;

// ReSharper disable once CheckNamespace

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
            CheckReentrancy();
            item.PropertyChanged -= OnItemPropertyChanged;
            item.PropertyChanged += OnItemPropertyChanged;
            base.InsertItem(GetInsertItemIndex(item), item);
        }

        /// <summary>
        ///     For items added in collection
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private int GetInsertItemIndex(T item)
        {
            if (Count == 0)
                return 0;

            for (var i = 0; i < Count; i++)
            {
                if (comparer.Compare(item, Items[i]) == 1)
                    continue;
                return i;
            }

            return Count;
        }

        /// <summary>
        ///     For elements that still in collection
        /// </summary>
        /// <param name="item"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private int GetReplaceItemIndex(T item, int index)
        {
            var prevIndex = index - 1;
            var nextIndex = index + 1;

            if (prevIndex == -1 && nextIndex == Count)
                return index;

            if (prevIndex == -1)
                return GetIndexIteratingForward(item, index);

            if (nextIndex == Count)
                return GetIndexIteratingBackward(item, index);

            var isIterFor = comparer.Compare(item, this[nextIndex]) == 1;
            var isIterBack = comparer.Compare(this[prevIndex], item) == 1;

            if (isIterFor && !isIterBack)
                return GetIndexIteratingForward(item, index);

            if (isIterBack && !isIterFor)
                return GetIndexIteratingBackward(item, index);

            Contract.Assert(!(isIterBack && isIterFor));

            return index;
        }

        private int GetIndexIteratingForward(T item, int index)
        {
            for (var i = index + 1; i < Count; ++i)
            {
                var compareResult = comparer.Compare(item, this[i]);
                if (compareResult == -1)
                    return i - 1;
            }

            return Count - 1;
        }

        private int GetIndexIteratingBackward(T item, int index)
        {
            for (var i = index - 1; i > -1; --i)
            {
                var compareResult = comparer.Compare(item, this[i]);
                if (compareResult == 1)
                    return i + 1;
            }
            return 0;
        }

        protected override void RemoveItem(int index)
        {
            this[index].PropertyChanged -= OnItemPropertyChanged;
            base.RemoveItem(index);
        }

        private void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnItemPropertyChanged((T)sender);
        }

        protected virtual void OnItemPropertyChanged(T item)
        {
            var index = IndexOf(item);
            var newIndex = GetReplaceItemIndex(item, index);

            if (newIndex != index)
                base.MoveItem(index, newIndex);
        }


        protected override sealed void SetItem(int index, T item)
        {
            throw new InvalidOperationException("You can't replace items by index property in sorted collection");
        }

        protected override sealed void MoveItem(int oldIndex, int newIndex)
        {
            throw new InvalidOperationException("Can't manualy move items in sorted collection");
        }

        protected override void ClearItems()
        {
            CheckReentrancy();
            foreach (var item in Items)
                item.PropertyChanged -= OnItemPropertyChanged;
            base.ClearItems();
        }
    }
}