using System;
using System.Windows.Input;

namespace MMK.Presentation.Tools
{
    public class Command : ICommand
    {
        public Command(Action execute, Func<bool> canExecute)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public Command(Action execute) : this(execute, () => true)
        {

        }

        private readonly Action execute;
        private readonly Func<bool> canExecute;

        public bool CanExecute(object parameter)
        {
            return canExecute();
        }

        public void Execute(object parameter)
        {
            execute();
        }

        public event EventHandler CanExecuteChanged;
    }

    public class Command<TArg> : ICommand
    {
        public Command(Action<TArg> execute, Func<bool> canExecute)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public Command(Action<TArg> execute)
            : this(execute, () => true)
        {
        }

        private readonly Action<TArg> execute;
        private readonly Func<bool> canExecute;

        public bool CanExecute(object parameter)
        {
            return canExecute();
        }

        public void Execute(object parameter)
        {
            execute((TArg)parameter);
        }

        public event EventHandler CanExecuteChanged;
    }
}
