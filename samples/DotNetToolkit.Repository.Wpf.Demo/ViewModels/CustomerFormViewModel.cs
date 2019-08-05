namespace DotNetToolkit.Repository.Wpf.Demo.ViewModels
{
    using DotNetToolkit.Wpf.Commands;
    using FluentValidation;
    using Infrastructure;
    using Infrastructure.Interfaces;
    using System;

    public class CustomerFormViewModel : FormViewModelBase<CustomerFormViewModelValidator>, ICustomer
    {
        #region Fields

        private int _id;
        private string _name;
        private string _notes;
        private DateTime? _date;

        #endregion

        #region Properties

        public int Id
        {
            get { return _id; }
            set { SetProperty(ref _id, value); }
        }

        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        public string Notes
        {
            get { return _notes; }
            set { SetProperty(ref _notes, value); }
        }

        public DateTime? Date
        {
            get { return _date; }
            set { SetProperty(ref _date, value); }
        }

        public RelayCommand SubmitCommand { get; private set; }

        #endregion

        #region Constructors

        public CustomerFormViewModel()
        {
            SubmitCommand = new RelayCommand(OnSubmitAsync);
        }

        #endregion
    }

    public class CustomerFormViewModelValidator : AbstractValidator<CustomerFormViewModel>
    {
        public CustomerFormViewModelValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("The field is required.");

            RuleFor(x => x.Date)
                .NotNull()
                .WithMessage("The field is required.");
        }
    }
}
