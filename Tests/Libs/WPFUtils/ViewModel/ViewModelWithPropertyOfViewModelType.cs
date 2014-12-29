namespace MMK.Wpf.ViewModel
{
    internal class ViewModelWithPropertyOfViewModelType : ViewModel
    {
        public SimpleViewModelWithStringProperty ChildViewModel { get; set; }
    }
}