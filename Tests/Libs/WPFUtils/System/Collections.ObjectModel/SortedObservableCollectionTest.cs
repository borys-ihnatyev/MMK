using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace System.Collections.ObjectModel
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
        [ExpectedException(typeof(ArgumentNullException))]
        public void ThrowsArgumentNullException_When_NullComparerPassed()
        {
            collection = new SortedObservableCollection<ObservbleInt32>(null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ThrowsArgumentNullException_WhenConstructWithEnumerable_NullComparerPassed()
        {
            collection = new SortedObservableCollection<ObservbleInt32>(CreateTestEnumerable(),null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ThrowsArgumentNullException_WhenConstructWithEnumerable_NullEnumerablePassed()
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
        public void RaiseCollectionChanged_WhenItemPropertyChanged()
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
        public void ResortCollection_WhenAdd()
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
        [ExpectedException(typeof(InvalidOperationException))]
        public void ThrowsInvalidOperatioException_When_AccessIndexerSetter()
        {
            collection.Add(100);
            collection.Add(200);
            collection.Add(300);

            collection[0] = 400;
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ThrowsInvalidOperatioException_When_Move()
        {
            collection.Add(100);    
            collection.Add(200);
            collection.Add(300);

            collection.Move(0,1);
        }

        [Test]
        public void WhenRemove_CountDecreased()
        {
            var observableInt32 = new ObservbleInt32(0);
            collection.Add(observableInt32);
            collection.Add(100);
            collection.Add(300);

            var initialCount = collection.Count;

            Assert.True(collection.Remove(observableInt32));
            Assert.AreEqual(initialCount - 1, collection.Count);
        }

        private void AssertCollectionIsSorted()
        {
            var array = collection.ToArray();
            Array.Sort(array, new ObservbleInt32.Comparer());

            for (var i = 0; i < array.Length; i++)
                Assert.AreEqual(array[i],collection[i]);
        }
    }
}