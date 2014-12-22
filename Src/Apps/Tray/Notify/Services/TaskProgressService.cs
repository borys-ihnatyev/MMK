using System;
using System.ComponentModel;
using System.Windows.Threading;
using MMK.ApplicationServiceModel;
using MMK.Notify.ViewModels;
using MMK.Notify.Views;

namespace MMK.Notify.Services
{
    public class TaskProgressService : Service
    {
        private TaskProgressView view;
        private readonly TaskProgressViewModel viewModel;

        public event EventHandler<ChangedEventArgs<bool>> StateChanged;

        public TaskProgressService()
        {
            viewModel = new TaskProgressViewModel();
        }

        protected override void OnInitialize()
        {
            viewModel.PropertyChanged += OnViewModelPropertyChanged;
            view = new TaskProgressView {DataContext = viewModel, Opacity = 0};
            view.BeginInit();
            view.Loaded += (s, e) => viewModel.LoadData();
            view.Closed += (s, e) => viewModel.UnloadData();
            view.EndInit();
            view.Show();
        }

        private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsActive")
                OnStateChanged();
        }

        public override void Start()
        {
            if (view.IsActive)
                return;

            Dispatcher.CurrentDispatcher.Invoke(delegate
            {
                view.Opacity = 1;
                view.Activate();
            });
            
        }

        public override void Stop()
        {
            Dispatcher.CurrentDispatcher.Invoke(delegate
            {
                view.Opacity = 0;
            });
        }

        protected virtual void OnStateChanged()
        {
            var handler = StateChanged;
            var state = viewModel.IsProgress;
            if (handler != null)
                handler(this, new ChangedEventArgs<bool>(!state, state));
        }
    }
}