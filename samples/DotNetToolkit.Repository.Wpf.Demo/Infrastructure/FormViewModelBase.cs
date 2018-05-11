namespace DotNetToolkit.Repository.Wpf.Demo.Infrastructure
{
    using DotNetToolkit.Wpf.Mvvm;
    using FluentValidation;
    using System;
    using System.Threading.Tasks;

    public abstract class FormViewModelBase<TValidator> : ViewModelBase where TValidator : IValidator
    {
        #region Fields

        private bool _isDirty;
        private readonly IValidator _validator;

        #endregion

        #region Properties

        public bool IsDirty
        {
            get { return _isDirty; }
            set { Set(ref _isDirty, value); }
        }

        #endregion

        #region Constructors

        protected FormViewModelBase()
        {
            _validator = Activator.CreateInstance<TValidator>();

            PropertyChanged += (sender, args) =>
            {
                if (!args.PropertyName.Equals(nameof(IsDirty)) &&
                    !args.PropertyName.Equals(nameof(IsInitialized)))
                {
                    IsDirty = true;
                }
            };
        }

        #endregion

        #region Implementation of IValidatable

        public virtual async Task<bool> ValidateAsync()
        {
            ClearErrors();

            var result = await _validator.ValidateAsync(this);
            if (result == null)
                return false;

            foreach (var error in result.Errors)
            {
                SetError(error.PropertyName, error.ErrorMessage);
            }

            return result.IsValid;
        }

        #endregion
    }
}
