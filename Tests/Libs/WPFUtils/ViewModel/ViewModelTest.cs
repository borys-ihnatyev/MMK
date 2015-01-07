using NUnit.Framework;

namespace MMK.Wpf.ViewModel
{
    [TestFixture]
    public class ViewModelTest
    {
        [Test]
        public void NotifyPropertyChangedRaisedWhenPropertyChanged()
        {
            var vm = new ViewModelWithStringProperty();
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
                ChildViewModel = new ViewModelWithStringProperty()
            };

            vm.LoadData();

            Assert.IsTrue(vm.ChildViewModel.IsDataLoaded);
        }

        [Test, ExpectedException(typeof (CommandNamingConventionException))]
        public void MustThrow_NameConventionException_When_CommandName_NotEndsWith_Command()
        {
            var vm = new ViewModelWithBadNamedCommand();
            vm.LoadData();
        }

        [Test]
        public void Command_MustBe_Null_When_CommandHandler_WasNotFounded()
        {
            var vm = new ViewModelWithCommandWithoutCommandHandler();
            vm.LoadData();
            Assert.IsNull(vm.TestCommand);
        }

        [Test]
        public void Command_MustBe_NotNull_When_Public_CommandHandlerWasFounded()
        {
            var vm = new ViewModelWithCommandAndPublicCommandHandler();
            vm.LoadData();
            Assert.IsNotNull(vm.TestCommand);
        }

        [Test]
        public void Command_MustBe_NotNull_When_Private_CommandHandlerWasFounded()
        {
            var vm = new ViewModelWithCommandAndPrivateCommandHandler();
            vm.LoadData();
            Assert.IsNotNull(vm.TestCommand);
        }

        [Test]
        public void Command_NotChangedByCommandLoader_When_IsManualySetup()
        {
            var vm = new ViewModelWithCommandAndPublicCommandHandler();

            var isLocalExecuted = false;

            var command = new Command(() => isLocalExecuted = true);
            vm.TestCommand = command;

            vm.LoadData();

            Assert.AreEqual(vm.TestCommand, command);
        }

        [Test, ExpectedException(typeof (CommandResolveException))]
        public void Throws_CommandResolveException_When_CommandHandler_HasManyOverloads()
        {
            var vm = new ViewModelWithCommandHandlerOverloads();
            vm.LoadData();
        }
    }
}