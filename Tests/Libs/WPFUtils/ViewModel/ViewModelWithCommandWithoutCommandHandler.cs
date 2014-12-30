using System.Windows.Input;

namespace MMK.Wpf.ViewModel
{
    public class ViewModelWithCommandWithoutCommandHandler : ViewModel
    {
        public ICommand TestCommand { get; set; }
    }
}