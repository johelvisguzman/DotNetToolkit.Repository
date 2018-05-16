namespace DotNetToolkit.Repository.Wpf.Demo.Infrastructure
{
    using System.Threading.Tasks;
    using System.Windows;
    using DotNetToolkit.Wpf.Metro.Dialogs;
    using MahApps.Metro.Controls;

    public class DialogController
    {
        #region Fields

        private static volatile DialogController _instance;
        private static readonly object _syncRoot = new object();

        private readonly MetroWindow _metroWindow;

        #endregion

        #region Properties

        public static DialogController Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_syncRoot)
                    {
                        if (_instance == null)
                            _instance = new DialogController();
                    }
                }

                return _instance;
            }
        }

        #endregion

        #region Constructors

        private DialogController()
        {
            _metroWindow = Application.Current.MainWindow as MetroWindow;
        }

        #endregion

        #region Public Methods

        public async Task<MessageDialogResult> ShowWarningMessageAsync(string message, MessageDialogStyle style = MessageDialogStyle.AffirmativeAndNegative)
        {
            var settings = new ChildWindowDialogSettings
            {
                AllowMove = true,
                TitleCharacterCasing = _metroWindow.TitleCharacterCasing,
                TitleBarBackground = (System.Windows.Media.Brush)Application.Current.Resources["FlatAlizarinColorBrush"]
            };

            return await _metroWindow.ShowChildWindowMessageAsync("Warning!", message, style, settings);
        }

        #endregion
    }
}
