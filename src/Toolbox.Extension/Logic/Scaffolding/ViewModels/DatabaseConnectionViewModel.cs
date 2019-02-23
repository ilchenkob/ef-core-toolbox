using System.Threading.Tasks;
using System.Windows.Input;
using Toolbox.Extension.Logic.DatabaseServices;
using Toolbox.Extension.Logic.Models;
using Toolbox.Extension.Logic.Scaffolding.DatabaseServices;
using Toolbox.Extension.UI.Services;

namespace Toolbox.Extension.Logic.Scaffolding.ViewModels
{
    public class DatabaseConnectionViewModel : BaseViewModel
    {
        private readonly IMessageBoxService _messageBoxService;
        private readonly IDatabaseConnector _dbConnector;
        // private readonly DatabaseTypes _databaseType = DatabaseTypes.MsSqlServer;

        public DatabaseConnectionViewModel(
            IMessageBoxService messageBoxService,
            IDatabaseConnector dbConnector)
        {
            _messageBoxService = messageBoxService;
            _dbConnector = dbConnector;

            Server =
            Database =
            Username =
            Password = string.Empty;

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

        public Task RaiseCantConnect() => _messageBoxService.ShowErrorMessage("Can not connect to database");

        private async Task testConnection()
        {
            var connectionString = GetConnectionString();
            var canConnect = await _dbConnector.TryConnect(connectionString);
            if (canConnect)
                await _messageBoxService.ShowInfoMessage("Succesfully connected");
            else
                await RaiseCantConnect();
        }
    }
}
