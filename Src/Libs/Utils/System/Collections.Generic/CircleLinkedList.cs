// ReSharper disable once CheckNamespace
namespace System.Collections.Generic
{
    /// <summary>
    /// CircleLinkedList not threadsafe container
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public partial class CircleLinkedList<T> : ICollection<T>
    {

        public CircleLinkedList()
        {
            Count = 0;
            first = null;
            last = null;
            current = null;
        }

        private Node<T> first;
        private Node<T> last;
        internal Node<T> current;

        public bool IsReadOnly
        {
            get { return false; }
        }

        public int Count { get; private set; }

        public Node<T> First
        {
            get { return first; }
        }
        public Node<T> Last
        {
            get { return last; }
        }
        public Node<T> Current
        {
            get { return current; }
        }

        public Node<T> MoveNext()
        {
            current = current.Next;
            return current;
        }

        public Node<T> MovePrevious()
        {
            current = current.Previous;
            return current;
        }

        public void Add(T item)
        {
            AddLast(item);
        }

        public void AddLast(T value)
        {
            AddLast(new Node<T>(value));
        }
        public void AddFirst(T value)
        {
            AddFirst(new Node<T>(value));
        }

        public void AddLast(Node<T> node)
        {
            CheckNodeListFree(node);
            if (Count > 0)
            {
                last.next = node;
                node.prev = last;
                node.next = first;
                first.prev = node;
                last = node;
            }
            else
            {
                first = node;
                last = first;
                first.next = first;
                first.prev = first;
                current = first;
            }
            ++Count;
            node.list = this;
        }

        public void AddFirst(Node<T> node)
        {
            CheckNodeListFree(node);
            if (Count > 0)
            {
                last.next = node;
                node.prev = last;
                node.next = first;
                first.prev = node;
                first = node;
            }
            else
            {
                first = node;
                last = first;
                first.next = first;
                first.prev = first;
                current = first;
            }
            node.list = this;
            ++Count;
        }

        private void CheckNodeListFree(Node<T> node)
        {
            if (node == null) throw new ArgumentNullException("node");
            if (node.list != null)
                throw new InvalidOperationException("Node is already binded with another List");
        }

        public Node<T> Find(T item) 
        {
            if (Count <= 0) return null;

            var node = first;
            do
            {
                if (node.Value.Equals(item))
                    return node;
                node = node.next;
            } while (node != first);

            return null;
        }

        public Node<T> FindLast(T item)
        {
            if (Count <= 0) return null;
            var node = last;
            do
            {
                if (node.Value.Equals(item))
                    return node;
                node = node.prev;
            } while (node != last);

            return null;
        }

        public bool Remove(T item)
        {
            if (Count <= 0) return false;
            var node = first;
            do
            {
                if (node.Value.Equals(item))
                {
                    Remove(node);
                    return true;
                }
                node = node.next;
            } while (node != first);

            return false;
        }

        private void Remove(Node<T> node)
        {
            CheckNodeList(node);

            var nodeToClear = node;

            node.next.prev = node.prev;
            node.prev.next = node.next;

            nodeToClear.Clear();

            --Count;
        }

        private void CheckNodeList(Node<T> node)
        {
            if (node == null) throw new ArgumentNullException("node");
            if (node.list != this)
                throw new InvalidOperationException("Node is already binded with another List");
        }

        public void Clear()
        {
            if (Count <= 0) return;

            var node = first;
            do
            {
                var nodeToClear = node;
                node = node.next;
                nodeToClear.Clear();
            } while (node != first);

            Count = 0;
            first = null;
            last = null;
            current = null;
        }

        public bool Contains(T item)
        {
            if (Count > 0)
            {
                var node = first;
                do
                {
                    if (node.Value.Equals(item))
                    {
                        return true;
                    }
                    node = node.next;
                } while (node != first);
            }
            return false;
        }
        
        public bool Contains(Node<T> node)
        {
            return node.list == this;
        }

        // ReSharper disable once LoopVariableIsNeverChangedInsideLoop
        public IEnumerator<T> GetEnumerator()
        {
            var node = current;
            do yield return current.Value;
            while (node != MoveNext());
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array.Length - arrayIndex < Count)
                throw new InvalidOperationException("Collection can't locate in this array, by size");

            var node = first;
            do
            {
                array[arrayIndex] = node.Value;
                ++arrayIndex;
                node = node.Next;
            } while (node != first);
        }
    }
}
