using System.Windows.Input;

namespace MMK.Wpf.ViewModel
{
    public class ViewModelWithBadNamedCommand : ViewModel
    {
        public ICommand BadCommandName { get; private set; }
    }
}