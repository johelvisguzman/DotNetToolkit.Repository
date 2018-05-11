namespace DotNetToolkit.Repository.Wpf.Demo.Infrastructure
{
    using DotNetToolkit.Wpf.Commands;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class NavigationController
    {
        #region Fields

        private static volatile NavigationController _instance;
        private static readonly object _syncRoot = new object();
        private readonly IList<object> _historic = new List<object>();
        private bool _navigatingBack;

        #endregion

        #region Events

        public event EventHandler<object> Navitgated;
        public event EventHandler<object> Navigating;

        #endregion

        #region Properties

        public static NavigationController Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_syncRoot)
                    {
                        if (_instance == null)
                            _instance = new NavigationController();
                    }
                }

                return _instance;
            }
        }

        public object Current { get; private set; }

        public TaskCompletionSource<bool> TaskCompletionSource { get; private set; }

        public RelayCommand<object> NavigateToCommand { get; private set; }

        public RelayCommand NavigateBackCommand { get; private set; }

        #endregion

        #region Constructors

        private NavigationController()
        {
            NavigateToCommand = new RelayCommand<object>(async (target) => await NavigateToAsync(target));
            NavigateBackCommand = new RelayCommand(async () => await NavigateBackAsync(), _ => _historic.Count > 1);
        }

        #endregion

        #region Public Methods

        public async Task NavigateBackAsync()
        {
            if (_historic.Count > 1)
            {
                var target = _historic.ElementAt(_historic.Count - 2);

                if (await WaitOnNavigatingCancelResponseAsync(target))
                    return;

                _navigatingBack = true;

                _historic.RemoveAt(_historic.Count - 1);
                Current = target;

                OnNavitgated(target);

                _navigatingBack = false;
            }
        }

        public Task NavigateToAsync<T>()
        {
            return NavigateToAsync(Activator.CreateInstance(typeof(T)));
        }

        public async Task NavigateToAsync(object target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            if (_navigatingBack)
                return;

            if (await WaitOnNavigatingCancelResponseAsync(target))
                return;

            _historic.Add(target);
            Current = target;

            OnNavitgated(target);
        }

        public void SetIsWaitingOnNavigatingResponse(bool isWaiting)
        {
            TaskCompletionSource = isWaiting ? new TaskCompletionSource<bool>() : null;
        }

        public void SetNavigatingResponse(bool response)
        {
            TaskCompletionSource?.SetResult(response);
        }

        #endregion

        #region Private Methods

        private Task<bool> WaitOnNavigatingCancelResponseAsync(object target)
        {
            Navigating?.Invoke(this, target);

            return TaskCompletionSource != null
                ? TaskCompletionSource.Task
                : Task.FromResult(false);
        }

        private void OnNavitgated(object target)
        {
            Navitgated?.Invoke(this, target);
        }

        #endregion
    }
}
