using Microsoft.VisualStudio.Shell;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Toolbox.Extension.Logic.DatabaseServices;
using Toolbox.Extension.Logic.Settings;
using Toolbox.Extension.Logic.Settings.Models;
using Toolbox.Extension.UI.Services;
using Task = System.Threading.Tasks.Task;

namespace Toolbox.Extension.Logic.Scaffolding.ViewModels
{
    public class DatabaseConnectionViewModel : BaseViewModel, IDisposable
    {
        private readonly IMessageBoxService _messageBoxService;
        private readonly IDatabaseConnector _dbConnector;
        // private readonly DatabaseTypes _databaseType = DatabaseTypes.MsSqlServer;

        private bool _isDisposing;
        private CancellationTokenSource _cancellationTokenSource;

        public DatabaseConnectionViewModel(
            IMessageBoxService messageBoxService,
            IDatabaseConnector dbConnector)
        {
            _messageBoxService = messageBoxService;
            _dbConnector = dbConnector;

            if (SettingsStore.Instance?.MsSqlDatabaseConnectionSettings != null &&
                !string.IsNullOrWhiteSpace(SettingsStore.Instance.MsSqlDatabaseConnectionSettings.Server))
            {
                Server = SettingsStore.Instance.MsSqlDatabaseConnectionSettings.Server;
                Database = SettingsStore.Instance.MsSqlDatabaseConnectionSettings.Database;
                Username = SettingsStore.Instance.MsSqlDatabaseConnectionSettings.Username;
                Password = SettingsStore.Instance.MsSqlDatabaseConnectionSettings.Password;
                IsSqlAuth = SettingsStore.Instance.MsSqlDatabaseConnectionSettings.IsSqlAuthSelected;

                RememberConnection = true;
            }
            else
            {
                Server =
                Database =
                Username =
                Password = string.Empty;
                RememberConnection = false;
            }

            TestConnectionCommand = new AsyncCommand(testConnection, () => IsValid);
        }

        public ICommand TestConnectionCommand { get; private set; }

        private string _server;
        public string Server
        {
            get { return _server; }
            set
            {
                _server = value;
                NotifyPropertyChanged(() => Server);
                NotifyPropertyChanged(() => IsValid);
                if (RememberConnection)
                    SettingsStore.Instance.MsSqlDatabaseConnectionSettings.Server = value;
            }
        }

        private string _database;
        public string Database
        {
            get { return _database; }
            set
            {
                _database = value;
                NotifyPropertyChanged(() => Database);
                NotifyPropertyChanged(() => IsValid);
                if (RememberConnection)
                    SettingsStore.Instance.MsSqlDatabaseConnectionSettings.Database = value;
            }
        }

        private string _username;
        public string Username
        {
            get { return _username; }
            set
            {
                _username = value;
                NotifyPropertyChanged(() => Username);
                NotifyPropertyChanged(() => IsValid);
                if (RememberConnection)
                    SettingsStore.Instance.MsSqlDatabaseConnectionSettings.Username = value;
            }
        }

        private string _password;
        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                NotifyPropertyChanged(() => Password);
                NotifyPropertyChanged(() => IsValid);
                if (RememberConnection)
                    SettingsStore.Instance.MsSqlDatabaseConnectionSettings.Password = value;
            }
        }

        private bool _isSqlAuth;
        public bool IsSqlAuth
        {
            get { return _isSqlAuth; }
            set
            {
                _isSqlAuth = value;
                NotifyPropertyChanged(() => IsSqlAuth);
                NotifyPropertyChanged(() => IsValid);

                if (RememberConnection)
                    SettingsStore.Instance.MsSqlDatabaseConnectionSettings.IsSqlAuthSelected = value;
            }
        }

        private bool _rememberConnection;
        public bool RememberConnection
        {
            get => _rememberConnection;
            set
            {
                if (value != _rememberConnection)
                {
                    if (!value)
                    {
                        SettingsStore.Instance.MsSqlDatabaseConnectionSettings = new MsSqlDatabaseConnectionSettings();
                    }
                    else
                    {
                        SettingsStore.Instance.MsSqlDatabaseConnectionSettings = new MsSqlDatabaseConnectionSettings
                        {
                            Server = Server,
                            Database = Database,
                            Username = Username,
                            Password = Password,
                            IsSqlAuthSelected = IsSqlAuth
                        };
                    }
                }

                _rememberConnection = value;
                NotifyPropertyChanged(() => RememberConnection);
            }
        }

        public bool IsValid
        {
            get
            {
                var credentialsValid = !IsSqlAuth ||
                    !(string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password));
                return !string.IsNullOrWhiteSpace(Server)
                    && !string.IsNullOrWhiteSpace(Database)
                    && credentialsValid;
            }
        }

        public string GetConnectionString()
        {
            return ConnectionStringBuilder.CreateMsSqlConnectionString(
                server: Server,
                database: Database,
                isSqlAuthentication: IsSqlAuth,
                username: Username,
                password: Password);
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                NotifyPropertyChanged(() => IsLoading);
            }
        }

        public Task RaiseCantConnect() => _messageBoxService.ShowErrorMessage("Can not connect to database");

        public void Dispose()
        {
            _isDisposing = true;

            try
            {
                _cancellationTokenSource?.Cancel(false);
            }
            catch (Exception) { }
        }

        private async Task testConnection()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            IsLoading = true;

            var connectionString = GetConnectionString();
            var canConnect = false;

            try
            {
                using (_cancellationTokenSource = new CancellationTokenSource(Constants.DefaultTaskTimeout))
                {
                    canConnect = await _dbConnector.TryConnect(connectionString, _cancellationTokenSource.Token);
                }
                _cancellationTokenSource = null;
            }
            catch (TaskCanceledException)
            {
                if (_isDisposing) return;
            }

            if (canConnect)
                await _messageBoxService.ShowInfoMessage("Succesfully connected");
            else if (!_isDisposing)
                await RaiseCantConnect();

            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            IsLoading = false;
        }
    }
}
