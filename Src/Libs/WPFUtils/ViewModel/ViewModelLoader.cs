using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MMK.Wpf.ViewModel
{
    internal class ViewModelLoader
    {
        private readonly ViewModel rootViewModel;

        private readonly List<PropertyInfo> declaredViewModelProperties;

        public ViewModelLoader(ViewModel rootViewModel)
        {
            this.rootViewModel = rootViewModel;

            declaredViewModelProperties = rootViewModel.GetType()
                .GetProperties()
                .Where(prop => prop.PropertyType.IsSubclassOf(typeof (ViewModel)))
                .ToList();
        }

        private IEnumerable<ViewModel> InitializedDeclaredViewModels
        {
            get
            {
                return declaredViewModelProperties
                    .Select(p => p.GetValue(rootViewModel))
                    .Where(v => !ReferenceEquals(v, null))
                    .OfType<ViewModel>();
            }
        }

        public void Load()
        {
            InitializedDeclaredViewModels.ForEach(vm => vm.LoadData());
        }

        public void Unload()
        {
            InitializedDeclaredViewModels.ForEach(vm => vm.UnloadData());
        }
    }
}