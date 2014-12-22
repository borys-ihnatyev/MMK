using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MMK.ApplicationServiceModel;
using MMK.Notify.Observer.Tasking.Observing;
using MMK.Notify.ViewModels;
using MMK.Notify.Views;

namespace MMK.Notify.Services
{
    public class TaskProgressService : Service
    {
        private Window view;
        private readonly TaskProgressViewModel viewModel;

        public TaskProgressService()
        {
            viewModel = new TaskProgressViewModel();
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
    }
}
