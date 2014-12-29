using System.Runtime.CompilerServices;
using NUnit.Core;
using NUnit.Framework;

namespace MMK.Wpf.ViewModel
{
    [TestFixture]
    public class ViewModelTest
    {
        [Test]
        public void NotifyPropertyChangedRaisedWhenPropertyChanged()
        {
            var vm = new SimpleViewModelWithStringProperty();
            vm.PropertyChanged += (s, e) => Assert.AreEqual(e.PropertyName, "Property");
            vm.Property = "Hello world";
        }

        [Test]
        public void LoadData_Of_ComplexViewModel_When_ChildViewModel_Is_Null()
        {
            var vm = new ViewModelWithPropertyOfViewModelType();
            vm.LoadData();
        }

        [Test]
        public void LoadData_Of_ComplexViewModel_When_ChildViewModel_Is_NotNull()
        {
            var vm = new ViewModelWithPropertyOfViewModelType
            {
                ChildViewModel = new SimpleViewModelWithStringProperty()
            };

            vm.LoadData();

            Assert.IsTrue(vm.ChildViewModel.IsDataLoaded);
        }


    }
}