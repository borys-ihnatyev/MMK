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

            Assert.AreEqual(0, collection[0]);
            Assert.AreEqual(1, collection[1]);
            Assert.AreEqual(2, collection[2]);
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

            Assert.AreEqual(0, collection[0]);
            Assert.AreEqual(100, collection[1]);
        }

        [Test]
        public void ResortCollection_When_ItemPropertyChanged()
        {
            var observableInt32 = new ObservbleInt32(0);
            collection.Add(observableInt32);
            collection.Add(100);
            collection.Add(300);

            observableInt32.Number = 200;

            Assert.AreEqual(100, collection[0]);
            Assert.AreEqual(200, collection[1]);
            Assert.AreEqual(300, collection[2]);
        }

        [Test]
        public void ResortCollection_When_ItemChanged()
        {
            collection.Add(100);
            collection.Add(200);
            collection.Add(300);

            collection[0] = 400;

            Assert.AreEqual(200, collection[0]);
            Assert.AreEqual(300, collection[1]);
            Assert.AreEqual(400, collection[2]);
        }
    }
}