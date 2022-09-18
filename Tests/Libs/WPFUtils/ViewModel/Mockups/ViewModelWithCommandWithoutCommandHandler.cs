using System.Windows.Input;

namespace MMK.Presentation.ViewModel.Mockups
{
    public class ViewModelWithCommandWithoutCommandHandler : ViewModel
    {
        public ICommand TestCommand { get; set; }
    }
}