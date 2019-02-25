using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Threading;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Toolbox.Extension.Logic.Models;
using Toolbox.Extension.Logic.DatabaseServices;
using Toolbox.Extension.Logic.Scaffolding.Models;
using Toolbox.Extension.UI.Services;
using Task = System.Threading.Tasks.Task;

namespace Toolbox.Extension.Logic.Scaffolding.ViewModels
{
    public class ScaffoldingWizardViewModel : BaseViewModel, IDisposable
    {
        private readonly IMessageBoxService _messageBoxService;
        private readonly IDatabaseService _dbService;
        private readonly IDatabaseConnector _dbConnector;
        private readonly IScaffoldingService _scaffoldingService;

        private bool _isDisposing;
        private string _connectionString;
        private CancellationTokenSource _cancellationTokenSource;

        public ScaffoldingWizardViewModel(
            IMessageBoxService messageBoxService,
            IDatabaseService dbService,
            IDatabaseConnector dbConnector,
            IScaffoldingService scaffoldingService,
            IReadOnlyCollection<Project> allSolutionProjects)
        {
            _messageBoxService = messageBoxService;
            _dbService = dbService;
            _dbConnector = dbConnector;
            _scaffoldingService = scaffoldingService;

            DatabaseConnectionVM = new DatabaseConnectionViewModel(messageBoxService, dbConnector);
            DatabaseConnectionVM.PropertyChanged += onValidationStateChanged;
            TablesVM = new TablesViewModel(_messageBoxService);
            TablesVM.PropertyChanged += onValidationStateChanged;
            OutputParamsVM = new OutputParamsViewModel(allSolutionProjects);
            OutputParamsVM.PropertyChanged += onValidationStateChanged;

            State = new WizardState(
                () => IsLoading,
                () => CurrentStepNumber,
                () => DatabaseConnectionVM.IsValid,
                () => TablesVM.IsValid,
                () => OutputParamsVM.IsValid
            );

            BackCommand = new Command(goBack);
            NextCommand = new AsyncCommand(goNext);
            OkCommand = new AsyncCommand(okExecute);
        }

        public DatabaseConnectionViewModel DatabaseConnectionVM { get; private set; }

        public TablesViewModel TablesVM { get; private set; }

        public OutputParamsViewModel OutputParamsVM { get; private set; }

        public Action CloseAction { get; set; }

        public ICommand BackCommand { get; private set; }

        public ICommand NextCommand { get; private set; }

        public ICommand OkCommand { get; private set; }

        private int _currentStepNumber;
        public int CurrentStepNumber
        {
            get { return _currentStepNumber; }
            private set
            {
                _currentStepNumber = value;
                NotifyPropertyChanged(() => CurrentStepNumber);
                NotifyPropertyChanged(() => State);
            }
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                NotifyPropertyChanged(() => IsLoading);
                NotifyPropertyChanged(() => State);
            }
        }

        public WizardState State { get; private set; }

        private void goBack()
        {
            --CurrentStepNumber;
        }

        private async Task goNext()
        {
            IsLoading = true;

            ++CurrentStepNumber;
            if (CurrentStepNumber == 1)
            {
                await ConnectAndGetTables();
            }

            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            IsLoading = false;
        }

        private async Task ConnectAndGetTables()
        {
            await TablesVM.ClearTables();

            _connectionString = DatabaseConnectionVM.GetConnectionString();

            var canConnect = false;
            try
            {
                using (_cancellationTokenSource = new CancellationTokenSource(Constants.DefaultTaskTimeout))
                {
                    canConnect = await _dbConnector.TryConnect(_connectionString, _cancellationTokenSource.Token);
                }
            }
            catch (TaskCanceledException) { }

            if (canConnect)
            {
                var hasTables = false;
                using (_cancellationTokenSource = new CancellationTokenSource(Constants.DefaultTaskTimeout))
                {
                    var tables = await _dbService.GetTables(_connectionString, _cancellationTokenSource.Token);
                    hasTables = await TablesVM.SetTables(tables);
                }
                if (hasTables)
                {
                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                    NotifyPropertyChanged(() => State);
                }
                else
                {
                    goBack();
                }
            }
            else if(!_isDisposing)
            {
                await RaiseCantConnect();
            }

            _cancellationTokenSource = null;
        }

        private async Task RaiseCantConnect()
        {
            await DatabaseConnectionVM.RaiseCantConnect();
            goBack();
        }

        private async Task okExecute()
        {
            var scaffoldingParams = new ScaffoldingExecutorParams
            {
                ConnectionString = _connectionString,
                Tables = TablesVM.GetSelectedTables(),
                DbContextNamespace = OutputParamsVM.Namespace,
                DbContextClassName = OutputParamsVM.ContextName,
                OutputFolder = OutputParamsVM.OutputPath,
                UseDataAnnotations = OutputParamsVM.UseDataAnnotations,
                UseDatabaseNames = OutputParamsVM.UseDbNames
            };
            _scaffoldingService.ScaffoldDatabase(scaffoldingParams);

            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            CloseAction?.Invoke();
        }

        private void onValidationStateChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == nameof(DatabaseConnectionVM.IsValid))
            {
                NotifyPropertyChanged(() => State);
            } else if (args.PropertyName == nameof(DatabaseConnectionVM.IsLoading))
            {
                IsLoading = DatabaseConnectionVM.IsLoading;
            }
        }

        public void Dispose()
        {
            _isDisposing = true;

            _cancellationTokenSource?.Cancel(false);

            DatabaseConnectionVM.PropertyChanged -= onValidationStateChanged;
            TablesVM.PropertyChanged -= onValidationStateChanged;
            OutputParamsVM.PropertyChanged -= onValidationStateChanged;

            DatabaseConnectionVM.Dispose();
        }
    }
}
