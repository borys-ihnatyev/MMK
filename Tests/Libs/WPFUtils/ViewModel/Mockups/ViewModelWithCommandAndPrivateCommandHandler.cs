namespace MMK.Presentation.ViewModel.Mockups
{
    public class ViewModelWithCommandAndPrivateCommandHandler : ViewModelWithCommandWithoutCommandHandler
    {
        private bool isTestExecuted;

        public bool IsTestExecuted
        {
            get { return isTestExecuted; }
            set
            {
                if (value == isTestExecuted) return;
                isTestExecuted = value;
                NotifyPropertyChanged();
            }
        }

        private void Test()
        {
            IsTestExecuted = true;
        }
    }
}