using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using MMK.MagicPlaylist.Base;
using MMK.Marking;
using NUnit.Framework;

namespace MMK.MagicPlaylist.ViewModels
{
    [TestFixture]
    public class MainViewModelTest : FileUsageTestFixture
    {
        private MainViewModel viewModel;

        [SetUp]
        public override void SetUp()
        {
            viewModel = new MainViewModel();
            base.SetUp();
        }

        [Test(Description = "Common")]
        public void Created()
        {
            Assert.IsNotNull(viewModel);
        }

        [Test(Description = "Common")]
        public void IsINotifyPropertyChanged()
        {
            Assert.IsTrue(viewModel is INotifyPropertyChanged);
        }

        [Test(Description = "Pattern")]
        public void Pattern_Is_NotNull_OnCreate()
        {
            Assert.IsNotNull(viewModel.Pattern);
        }

        [Test(Description = "Pattern")]
        public void PatternString_Is_NotNull_OnCreate()
        {
            Assert.IsNotNull(viewModel.PatternString);
        }

        [Test(Description = "Pattern")]
        public void Cant_Add_HashTags_Via_Pattern()
        {
            viewModel.Pattern.Add(new HashTag("test"));
            Assert.AreEqual(0, viewModel.Pattern.Count);
        }

        [Test(Description = "Pattern")]
        [TestCase(1, "#test")]
        [TestCase(1, "#test dkdfhld")]
        [TestCase(2, "#test #dkdfhld")]
        public void Can_Add_HashTags_Via_PatternString(int expectedCount, string patternString)
        {
            viewModel.PatternString = patternString;
            Assert.AreEqual(expectedCount, viewModel.Pattern.Count);
        }

        [Test(Description = "Pattern")]
        [TestCase("#amoll")]
        [TestCase("#amoll #test")]
        [TestCase("#test #amoll")]
        public void Errors_When_Pattern_Has_KeyHashTag(string patternString)
        {
            viewModel.PatternString = patternString;
            Assert.IsTrue(viewModel["Pattern"] == "Can't hold key");
        }
    }
}