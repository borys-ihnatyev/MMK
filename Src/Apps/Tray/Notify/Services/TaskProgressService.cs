using System;
using System.ComponentModel;
using MMK.ApplicationServiceModel;
using MMK.Notify.ViewModels;
using MMK.Notify.Views;

namespace MMK.Notify.Services
{
    public class TaskProgressService : Service
    {
        private TaskProgressView view;
        private readonly TaskProgressViewModel viewModel;

        public TaskProgressService()
        {
            viewModel = new TaskProgressViewModel();
            viewModel.PropertyChanged += OnViewModelPropertyChanged;
        }

        public bool IsActive
        {
            get { return viewModel.IsActive; }
        }

        private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "IsActive")
                OnIsActiveChanged();
        }

        protected override void OnInitialize()
        {
            view = new TaskProgressView {DataContext = viewModel};
            view.BeginInit();
            view.Loaded += (s, e) => viewModel.LoadData();
            view.Closed += (s, e) => viewModel.UnloadData();
            view.EndInit();
        }

        public override void Start()
        {
            view.Show();
        }

        public override void Stop()
        {
            view.Hide();
        }

        public event EventHandler<ChangedEventArgs<bool>> IsActiveChanged;

        protected virtual void OnIsActiveChanged()
        {
            var handler = IsActiveChanged;
            if (handler != null) 
                handler(this, new ChangedEventArgs<bool>(!IsActive, IsActive));
        }
    }
}