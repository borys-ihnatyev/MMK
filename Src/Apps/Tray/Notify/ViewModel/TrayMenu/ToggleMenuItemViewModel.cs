namespace MMK.Notify.ViewModel.TrayMenu
{
    public class ToggleMenuItemViewModel : MenuItemViewModel
    {
        private readonly MenuItemViewModel initialState;
        private readonly MenuItemViewModel pressedState;
        private bool isPressed;

        public ToggleMenuItemViewModel(MenuItemViewModel initialState, MenuItemViewModel pressedState)
        {
            this.initialState = initialState;
            this.initialState.CommandAction += ChangeStateAction;
            this.pressedState = pressedState;
            this.pressedState.CommandAction += ChangeStateAction;
            ChangeState(this.initialState);
        }

        public bool IsPressed
        {
            get { return isPressed; }
            private set
            {
                isPressed = value;
                NotifyPropertyChanged();
            }
        }

        private void ChangeStateAction()
        {
            ChangeState(IsPressed ? initialState : pressedState);
            IsPressed = !IsPressed;
        }

        private void ChangeState(MenuItemViewModel state)
        {
            Image = state.Image;
            Title = state.Title;
            Command = state.Command;
        }

        public void Press()
        {
            Command.Execute(null);
        }
    }
}