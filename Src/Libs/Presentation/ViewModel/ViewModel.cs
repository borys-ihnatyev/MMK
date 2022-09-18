using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MMK.Presentation.ViewModel
{
    public abstract class ViewModel : ObservableObject, IDataErrorInfo
    {
        private readonly ViewModelLoader viewModelsLoader;
        private readonly CommandLoader commandLoader;

        protected ViewModel()
        {
            viewModelsLoader = new ViewModelLoader(this);
            commandLoader = new CommandLoader(this);
        }

        public bool IsDataLoaded { get; private set; }

        public void LoadData()
        {
            if (IsDataLoaded) return;

            OnLoadData();
            
            viewModelsLoader.Load();
            commandLoader.BindCommands();

            IsDataLoaded = true;
        }

        protected virtual void OnLoadData()
        {
        }


        public void UnloadData()
        {
            if (!IsDataLoaded) return;

            OnUnloadData();
            viewModelsLoader.Unload();

            IsDataLoaded = false;
        }

        protected virtual void OnUnloadData()
        {
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
            get { throw new NotSupportedException(); }
        }

        #endregion
    }
}