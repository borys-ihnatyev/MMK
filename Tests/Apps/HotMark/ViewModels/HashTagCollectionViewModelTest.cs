using NUnit.Framework;

namespace MMK.HotMark.ViewModels
{
    [TestFixture]
    public class HashTagCollectionViewModelTest
    {
        private HashTagCollectionViewModel hashTags;

        [SetUp]
        public void SetUp()
        {
            hashTags = new HashTagCollectionViewModel();
        }

        [Test]
        public void Selected_MustBe_Null_When_Created()
        {
            Assert.IsNull(hashTags.Selected);
        }

        [Test]
        public void Selected_MustBe_FirstAddedItem()
        {
            var hashTag = new HashTagViewModel("test");
            hashTags.Add(hashTag);
            Assert.AreEqual(hashTag,hashTags.Selected);
        }

        [Test]
        public void Selected_MustBe_FirstAddedItem_After_AddAnotherNotEmptyItem()
        {
            var hashTag = new HashTagViewModel("test");
            hashTags.Add(hashTag);
            hashTags.Add(new HashTagViewModel("test2"));
            Assert.AreEqual(hashTag, hashTags.Selected);    
        }

        [Test]
        public void Selected_MustBe_Null_When_RemovedLast()
        {
            var hashTag = new HashTagViewModel("test");
            hashTags.Add(hashTag);
            hashTags.Remove(hashTag);
            Assert.AreEqual(null, hashTags.Selected);
        }

        [Test]
        public void Selected_MustBe_Null_When_Cleared()
        {
            var hashTag = new HashTagViewModel("test");
            hashTags.Add(hashTag);
            hashTags.Clear();
            Assert.AreEqual(null, hashTags.Selected);
        }

        [Test]
        public void Selected_MustBe_EmptyItem_When_AddEmptyItem_On_NotEmptyCollection_With_NotNullSelected()
        {
            hashTags.Add(new HashTagViewModel("test1"));
            hashTags.Add(new HashTagViewModel("test2"));
            hashTags.Add(new HashTagViewModel("test3"));

            hashTags.Add();

            Assert.IsTrue(hashTags.Selected.IsEmpty);
        }

        [Test]
        public void Selected_MustBe_Null_When_RemovedLastItem()
        {
            hashTags.Add();
            hashTags.Add(new HashTagViewModel("test1"));
            hashTags.Add(new HashTagViewModel("test2"));

            hashTags.Clear();

            Assert.IsNull(hashTags.Selected);
        }

        [Test]
        public void Selected_MustBe_NextInCollection_When_RemovedSelected_InCollectionOfMoreThan2()   
        {
            var firstAdded = new HashTagViewModel("test");
            hashTags.Add(firstAdded);
            hashTags.Add(new HashTagViewModel("test3"));
            hashTags.Add(new HashTagViewModel("test2"));
            hashTags.Remove(firstAdded);

            Assert.IsNotNull(hashTags.Selected);
            Assert.AreNotSame(firstAdded,hashTags.Selected);
        }

        [Test]
        public void Count_IsNotChanged_When_AddNew_On_CollectionWithEmptyHashTag()
        {
            hashTags.Add(new HashTagViewModel());

            hashTags.Add();
            hashTags.Add();

            Assert.AreEqual(1,hashTags.Count);
        }

        [Test]
        public void ItemChangedToEmpty_MustCause_RemoveOfEmptyItem_When_CollectionHadEmptyItem()
        {
            var emptyItem = new HashTagViewModel();
            hashTags.Add(emptyItem);

            var itemChangedToEmpty = new HashTagViewModel("test");
            hashTags.Add(itemChangedToEmpty);

            itemChangedToEmpty.HashTagValue = "";

            Assert.AreEqual(-1,hashTags.IndexOf(emptyItem));
        }
    }
}