namespace DotNetToolkit.Repository.Wpf.Demo.Views
{
    /// <summary>
    /// Interaction logic for CustomerWorkspaceView.xaml
    /// </summary>
    public partial class CustomerWorkspaceView
    {
        public CustomerWorkspaceView()
        {
            InitializeComponent();

            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, System.Windows.RoutedEventArgs e)
        {
            DataGrid.Items.Refresh();
        }
    }
}
