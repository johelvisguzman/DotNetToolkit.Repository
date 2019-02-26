namespace DotNetToolkit.Repository.Wpf.Demo.ViewModels
{
    using DotNetToolkit.Wpf.Commands;
    using DotNetToolkit.Wpf.Metro.Dialogs;
    using DotNetToolkit.Wpf.Mvvm;
    using Factories;
    using Infrastructure;
    using Models;
    using Services;
    using System;
    using System.Collections.ObjectModel;

    public class CustomerWorkspaceViewModel : ViewModelBase
    {
        #region Fields

        private readonly IServiceLocator _serviceLocator;
        private ObservableCollection<CustomerFormViewModel> _customers;
        private readonly NavigationController _navigator;

        private readonly IRepositoryFactory _repositoryFactory;

        #endregion

        #region Properties

        public ObservableCollection<CustomerFormViewModel> Customers
        {
            get { return _customers; }
            private set { Set(ref _customers, value); }
        }

        public RelayCommand AddCommand { get; private set; }

        public RelayCommand<CustomerFormViewModel> EditCommand { get; private set; }

        public RelayCommand<CustomerFormViewModel> DeleteCommand { get; private set; }

        #endregion

        #region Constructors

        public CustomerWorkspaceViewModel(IServiceLocator serviceLocator)
        {
            AddCommand = new RelayCommand(OnAdd);
            EditCommand = new RelayCommand<CustomerFormViewModel>(OnEdit);
            DeleteCommand = new RelayCommand<CustomerFormViewModel>(OnDelete);
            DisplayName = "Customers";

            _serviceLocator = serviceLocator;
            _navigator = NavigationController.Instance;
            _repositoryFactory = serviceLocator.GetService<IRepositoryFactory>();
        }

        #endregion

        #region Private Methods

        private async void OnAdd()
        {
            var viewModel = _serviceLocator.GetService<CustomerFormViewModel>();

            viewModel.DisplayName = "New Customer";
            viewModel.Date = DateTime.Now;
            viewModel.IsDirty = false;
            viewModel.Submitted += async (sender, e) =>
            {
                var repo = _repositoryFactory.Create<Customer>();
                var model = new Customer();

                AutoMapper.Map(viewModel, model);

                repo.Add(model);

                viewModel.Id = model.Id;
                viewModel.IsDirty = false;

                Refresh();

                await _navigator.NavigateBackAsync();
            };

            SetNavigationRestriction(viewModel);

            await _navigator.NavigateToAsync(viewModel);
        }

        private async void OnEdit(CustomerFormViewModel viewModel)
        {
            if (viewModel == null)
                return;

            var copy = AutoMapper.Map<CustomerFormViewModel, CustomerFormViewModel>(viewModel);

            copy.DisplayName = "Edit Customer";
            copy.IsDirty = false;
            copy.Submitted += async (sender, e) =>
            {
                AutoMapper.Map(copy, viewModel);

                var repo = _repositoryFactory.Create<Customer>();
                var model = repo.Find(viewModel.Id);

                AutoMapper.Map(viewModel, model);

                repo.Update(model);

                copy.IsDirty = viewModel.IsDirty = false;

                Refresh();

                await _navigator.NavigateBackAsync();
            };

            SetNavigationRestriction(copy);

            await _navigator.NavigateToAsync(copy);
        }

        private async void OnDelete(CustomerFormViewModel viewModel)
        {
            if (viewModel == null)
                return;

            if (await DialogController.Instance.ShowWarningMessageAsync(
                    $"Do you wish to delete item '{viewModel.Id}'?") == MessageDialogResult.Affirmative)
            {
                var repo = _repositoryFactory.Create<Customer>();

                repo.Delete(viewModel.Id);

                Refresh();
            }
        }

        private void SetNavigationRestriction(CustomerFormViewModel viewModel)
        {
            EventHandler<object> navigating = null;

            navigating = async (sender, e) =>
            {
                var isWaiting = false;

                if (viewModel.IsDirty && (viewModel.Id != 0 || viewModel.IsInitialized))
                {
                    isWaiting = await DialogController.Instance.ShowWarningMessageAsync(
                        "The form has not been saved. Do you wish to continue?") == MessageDialogResult.Negative;

                    _navigator.SetNavigatingResponse(isWaiting);
                }

                _navigator.SetIsWaitingOnNavigatingResponse(isWaiting);
            };

            EventHandler<object> navigated = null;

            navigated = (sender, e) =>
            {
                if (e.ToString().Equals(typeof(CustomerFormViewModel).FullName))
                {
                    _navigator.SetIsWaitingOnNavigatingResponse(true);
                }
                else
                {
                    _navigator.Navigating -= navigating;
                    _navigator.Navitgated -= navigated;
                }
            };

            _navigator.Navigating += navigating;
            _navigator.Navitgated += navigated;
        }

        private void Refresh()
        {
            var repo = _repositoryFactory.Create<Customer>();
            var data = repo.FindAll<CustomerFormViewModel>(x => AutoMapper.Map<Customer, CustomerFormViewModel>(x));

            Customers = new ObservableCollection<CustomerFormViewModel>(data);
        }

        #endregion
    }
}
