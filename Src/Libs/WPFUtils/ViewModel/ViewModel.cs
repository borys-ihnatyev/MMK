using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace MMK.Wpf.ViewModel
{
    public abstract class ViewModel : ObservableObject, IDataErrorInfo
    {
        public bool IsDataLoaded { get; set; }

        public void LoadData()
        {
            if(IsDataLoaded) return;

            OnLoadData();
            LoadDeclaredViewModels();
            IsDataLoaded = true;
        }

        private IEnumerable<ViewModel> GetDeclaredViewModels()
        {
            return GetType()
                .GetProperties()
                .Where(prop => prop.PropertyType.IsSubclassOf(typeof (ViewModel)))
                .Select(prop => prop.GetMethod.Invoke(this, null))
                .OfType<ViewModel>();
        }

        protected virtual void OnLoadData()
        {
            
        }

        private void LoadDeclaredViewModels()
        {
            GetDeclaredViewModels().ForEach(vm => vm.LoadData());
        }

        public void UnloadData()
        {
            if(!IsDataLoaded) return;

            OnUnloadData();
            UnloadDeclaredViewModels();
            IsDataLoaded = false;
        }

        protected virtual void OnUnloadData()
        {

        }

        private void UnloadDeclaredViewModels()
        {
            GetDeclaredViewModels().ForEach(vm => vm.UnloadData());
        }

        #region IDataErrorInfo

        public string this[string columnName]
        {
            get { return OnValidation(columnName); }
        }

        protected virtual string OnValidation(string columnName)
        {
            var errors = new Collection<ValidationResult>();
            
            var validationContext = new ValidationContext(this);
            
            return Validator.TryValidateProperty(columnName, validationContext, errors) 
                ? null 
                : errors[0].ErrorMessage;
        }

        public string Error
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        #endregion
    }
}
