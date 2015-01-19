using System;
using System.ComponentModel;
using MMK.ApplicationServiceModel;
using MMK.Notify.ViewModels;
using MMK.Notify.Views;

namespace MMK.Notify.Services
{
    public class TaskProgressService : InitializableService
    {
        private TaskProgressView taskProgressView;
        private readonly TaskProgressViewModel taskProgressViewModel;

        public event EventHandler<ChangedEventArgs<bool>> StateChanged;

        public TaskProgressService()
        {
            taskProgressViewModel = new TaskProgressViewModel();
        }

        protected override void OnInitialize()
        {
            taskProgressViewModel.PropertyChanged += OnTaskProgressViewModelPropertyChanged;
            taskProgressViewModel.LoadData();

            taskProgressView = new TaskProgressView {DataContext = taskProgressViewModel};
            taskProgressView.Closed += (s, e) => taskProgressViewModel.UnloadData();
            taskProgressView.Show();
        }

        private void OnTaskProgressViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsProgress")
                OnStateChanged();
        }

        protected override void OnStart()
        {
            if (!taskProgressViewModel.IsProgress)
                return;

            taskProgressViewModel.IsVisible = true;
            taskProgressView.Activate();
        }

        protected override void OnStop()
        {
            taskProgressViewModel.IsVisible = false;
        }

        protected virtual void OnStateChanged()
        {
            var handler = StateChanged;
            var state = taskProgressViewModel.IsProgress;
            if (handler != null)
                handler(this, new ChangedEventArgs<bool>(!state, state));
        }
    }
}