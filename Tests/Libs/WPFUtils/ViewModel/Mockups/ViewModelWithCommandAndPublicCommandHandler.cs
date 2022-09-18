namespace MMK.Presentation.ViewModel.Mockups
{
    public class ViewModelWithCommandAndPublicCommandHandler : ViewModelWithCommandWithoutCommandHandler
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

        public void Test()
        {
            isTestExecuted = true;
        }
    }
}