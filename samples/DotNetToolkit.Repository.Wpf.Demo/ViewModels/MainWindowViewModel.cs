namespace DotNetToolkit.Repository.Wpf.Demo.ViewModels
{
    using DotNetToolkit.Wpf.Mvvm;
    using Infrastructure;
    using Services;
    using System.Collections.Generic;
    using System.Linq;

    public class MainWindowViewModel : ViewModelBase
    {
        #region Fields

        private object _activeItem;

        #endregion

        #region Properties

        public NavigationController Navigator { get; } = NavigationController.Instance;

        public object ActiveItem
        {
            get { return _activeItem; }
            set { SetProperty(ref _activeItem, value); }
        }

        public IEnumerable<ViewModelBase> Workspaces { get; private set; }

        #endregion

        #region Constructors

        public MainWindowViewModel(IServiceLocator serviceLocator)
        {
            Workspaces = new ViewModelBase[]
            {
                serviceLocator.GetService<CustomerWorkspaceViewModel>()
            };
        }

        #endregion

        #region Overrides of ViewModelBase

        protected override async void OnInitialize()
        {
            Navigator.Navitgated += (sender, e) => ActiveItem = e;

            await Navigator.NavigateToAsync(Workspaces.First());
        }

        #endregion
    }
}
