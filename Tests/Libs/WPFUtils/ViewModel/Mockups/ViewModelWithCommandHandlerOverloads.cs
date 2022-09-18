using System.Windows.Input;

namespace MMK.Presentation.ViewModel.Mockups
{
    public class ViewModelWithCommandHandlerOverloads : ViewModel
    {
        public bool IsTestExecuted { get; private set; }

        public ICommand TestCommand { get; private set; }

        public void Test()
        {
            IsTestExecuted = true;
        }

        public void Test(bool isExecuted)
        {
            IsTestExecuted = isExecuted;
        }
    }
}