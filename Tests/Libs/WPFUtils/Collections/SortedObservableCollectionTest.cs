using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using NUnit.Framework;

namespace MMK.Presentation.Collections
{
    [TestFixture]
    public class SortedObservableCollectionTest
    {
        private SortedObservableCollection<ObservbleInt32> collection;

        [SetUp]
        public void SetUp()
        {
            collection = new SortedObservableCollection<ObservbleInt32>(new ObservbleInt32.Comparer());
        }

        [Test]
        [ExpectedException(typeof (ArgumentNullException))]
        public void ThrowsArgumentNullException_When_NullComparerPassed()
        {
            collection = new SortedObservableCollection<ObservbleInt32>(null);
        }

        [Test]
        [ExpectedException(typeof (ArgumentNullException))]
        public void ThrowsArgumentNullException_When_ConstructWithEnumerable_NullComparerPassed()
        {
            collection = new SortedObservableCollection<ObservbleInt32>(CreateTestEnumerable(), null);
        }

        [Test]
        [ExpectedException(typeof (ArgumentNullException))]
        public void ThrowsArgumentNullException_When_ConstructWithEnumerable_NullEnumerablePassed()
        {
            collection = new SortedObservableCollection<ObservbleInt32>(null, null);
        }

        [Test]
        public void CreateCollectionFrom_IEnumerable()
        {
            var intEnumerable = CreateTestEnumerable();
            collection = new SortedObservableCollection<ObservbleInt32>(intEnumerable, new ObservbleInt32.Comparer());

            AssertCollectionIsSorted();
        }

        private static IEnumerable<ObservbleInt32> CreateTestEnumerable()
        {
            return Enumerable.Range(0, 3).Select(i => (ObservbleInt32) i).Reverse();
        }

        [Test]
        public void RaiseCollectionChanged_When_ItemPropertyChanged()
        {
            var observableInt32 = new ObservbleInt32(0);
            collection.Add(observableInt32);
            collection.Add(100);

            var wasRaised = false;
            collection.CollectionChanged += (s, e) => wasRaised = true;

            observableInt32.Number = 300;

            Assert.IsTrue(wasRaised);
        }

        [Test]
        public void ResortCollection_When_Add()
        {
            collection.Add(100);
            collection.Add(0);

            AssertCollectionIsSorted();
        }

        [Test]
        public void ResortCollectionOf3_When_FirstItemPropertyChanged()
        {
            var observableInt32 = new ObservbleInt32(0);
            collection.Add(observableInt32);
            collection.Add(100);
            collection.Add(300);

            observableInt32.Number = 200;

            AssertCollectionIsSorted();
        }

        [Test]
        public void ResortCollectionOf3_When_LastItemPropertyChanged()
        {
            collection.Add(100);
            collection.Add(300);
            var observableInt32 = new ObservbleInt32(400);
            collection.Add(observableInt32);

            observableInt32.Number = 200;

            AssertCollectionIsSorted();
        }

        [Test]
        public void ResortCollectionOf3_When_MiddleItemPropertyChanged_WithHigh()
        {
            collection.Add(100);
            var observableInt32 = new ObservbleInt32(200);
            collection.Add(observableInt32);
            collection.Add(300);

            observableInt32.Number = 400;

            AssertCollectionIsSorted();
        }

        [Test]
        public void ResortCollectionOf3_When_MiddleItemPropertyChanged_WithLow()
        {
            collection.Add(100);
            var observableInt32 = new ObservbleInt32(200);
            collection.Add(observableInt32);
            collection.Add(300);

            observableInt32.Number = 50;

            AssertCollectionIsSorted();
        }

        [Test]
        public void ResortCollectionOf3_When_MiddleItemPropertyChanged()
        {
            collection.Add(100);
            var observableInt32 = new ObservbleInt32(200);
            collection.Add(observableInt32);
            collection.Add(300);

            observableInt32.Number = 250;

            AssertCollectionIsSorted();
        }

        [Test]
        public void ResortCollectionOf1_When_ItemPropertyChanged()
        {
            var observableInt32 = new ObservbleInt32(0);
            collection.Add(observableInt32);

            observableInt32.Number = 200;

            Assert.AreEqual(200, collection[0]);
        }

        [Test]
        [ExpectedException(typeof (InvalidOperationException))]
        public void ThrowsInvalidOperatioException_When_AccessIndexerSetter()
        {
            collection.Add(100);
            collection.Add(200);
            collection.Add(300);

            collection[0] = 400;
        }

        [Test]
        [ExpectedException(typeof (InvalidOperationException))]
        public void ThrowsInvalidOperatioException_When_Move()
        {
            collection.Add(100);
            collection.Add(200);
            collection.Add(300);

            collection.Move(0, 1);
        }

        [Test]
        public void CountDecreased_When_Remove()
        {
            var observableInt32 = new ObservbleInt32(0);
            collection.Add(observableInt32);
            collection.Add(100);
            collection.Add(300);

            var initialCount = collection.Count;

            Assert.True(collection.Remove(observableInt32));
            Assert.AreEqual(initialCount - 1, collection.Count);
        }

        [Test]
        public void NotRaiseCollectionChanged_When_ChangeRemovedElement()
        {
            var observable = new ObservbleInt32(12);

            collection.Add(observable);
            collection.Remove(observable);

            var wasRaised = false;
            collection.CollectionChanged += (s, e) => wasRaised = true;

            observable.Number = 100;

            Assert.IsFalse(wasRaised);
        }

        [Test(Description = "Event behavior")]
        public void NotRaiseCollectionChanged_After_ClearNotEmptyCollection_When_ChangeRemovedElement()
        {
            var observable = new ObservbleInt32(12);
            var observable2 = new ObservbleInt32(13);

            collection.Add(observable);
            collection.Add(observable2);
            collection.Clear();

            var wasRaised = false;
            collection.CollectionChanged += (s, e) => wasRaised = true;

            observable.Number = 100;

            Assert.IsFalse(wasRaised);
        }

        [Test(Description = "Event behavior")]
        public void CollectionChangedAddAction_RaisedOnce_When_InsertOneItem()
        {
            var addCount = 0;
            var otherCount = 0;

            collection.Add(10);
            collection.Add(9);

            collection.CollectionChanged += (s, e) =>
            {
                if (e.Action == NotifyCollectionChangedAction.Add)
                    ++addCount;
                else
                    ++otherCount;
            };

            collection.Add(12);

            Assert.AreEqual(1, addCount);
            Assert.AreEqual(0, otherCount);
        }

        [Test(Description = "Event behavior")]
        public void CollectionChangedMoveAction_NotRaised_When_UpdateOneItem_Witout_ExpectedChange()
        {
            var moveCount = 0;
            var otherCount = 0;

            var observable = new ObservbleInt32(12);
            collection.Add(observable);
            collection.Add(13);
            collection.Add(14);
            collection.CollectionChanged += (s, e) =>
            {
                if (e.Action == NotifyCollectionChangedAction.Move)
                    ++moveCount;
                else
                    ++otherCount;
            };

            observable.Number = 10;

            Assert.AreEqual(0, moveCount);
            Assert.AreEqual(0, otherCount);
        }

        [Test(Description = "Event behavior")]
        public void CollectionChangedMoveAction_RaisedOnce_When_UpdateItem_With_ExpectedChange()
        {
            var moveCount = 0;
            var otherCount = 0;

            var observable = new ObservbleInt32(12);
            collection.Add(observable);
            collection.Add(13);
            collection.Add(14);
            collection.CollectionChanged += (s, e) =>
            {
                if (e.Action == NotifyCollectionChangedAction.Move)
                    ++moveCount;
                else
                    ++otherCount;
            };

            observable.Number = 16;

            Assert.AreEqual(1, moveCount);
            Assert.AreEqual(0, otherCount);
        }

        [Test(Description = "Event behavior")]
        public void CollectionChangedRemoveAction_RaisedOnce_When_RemoveItem()
        {
            var removeCount = 0;
            var otherCount = 0;

            var observable = new ObservbleInt32(12);
            collection.Add(observable);
            collection.Add(13);
            collection.Add(14);
            collection.CollectionChanged += (s, e) =>
            {
                if (e.Action == NotifyCollectionChangedAction.Remove)
                    ++removeCount;
                else
                    ++otherCount;
            };

            collection.Remove(observable);

            Assert.AreEqual(1, removeCount);
            Assert.AreEqual(0, otherCount);
        }

        [Test(Description = "Event behavior")]
        public void CollectionChangedResetAction_RaisedOnce_When_ClearItems()
        {
            var clearCount = 0;
            var otherCount = 0;

            collection.Add(13);
            collection.Add(11);
            collection.Add(12);
            collection.Add(14);

            collection.CollectionChanged += (s, e) =>
            {
                if (e.Action == NotifyCollectionChangedAction.Reset)
                    ++clearCount;
                else
                    ++otherCount;
            };

            collection.Clear();

            Assert.AreEqual(1, clearCount);
            Assert.AreEqual(0, otherCount);
        }

        private void AssertCollectionIsSorted()
        {
            var array = collection.ToArray();
            Array.Sort(array, new ObservbleInt32.Comparer());

            for (var i = 0; i < array.Length; i++)
                Assert.AreEqual(array[i], collection[i]);
        }
    }
}