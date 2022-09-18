using System.Windows.Input;

namespace MMK.Presentation.ViewModel.Mockups
{
    public class ViewModelWithBadNamedCommand : ViewModel
    {
        public ICommand BadCommandName { get; private set; }
    }
}