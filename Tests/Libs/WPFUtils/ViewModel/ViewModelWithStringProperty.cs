namespace MMK.Wpf.ViewModel
{
    internal class ViewModelWithStringProperty : ViewModel
    {
        private string property;

        public string Property
        {
            get { return property; }
            set
            {
                if (value == property) return;
                property = value;
                NotifyPropertyChanged();
            }
        }
    }
}