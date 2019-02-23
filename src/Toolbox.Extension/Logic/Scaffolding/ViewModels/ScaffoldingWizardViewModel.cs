using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Threading;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using Toolbox.Extension.Logic.Models;
using Toolbox.Extension.Logic.Scaffolding;
using Toolbox.Extension.Logic.Scaffolding.DatabaseServices;
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

        private string _connectionString;

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
        
        public WizardState State { get; private set; }

        private void goBack()
        {
            --CurrentStepNumber;
        }

        private async Task goNext()
        {
            ++CurrentStepNumber;
            if (CurrentStepNumber == 1)
            {
                _connectionString = DatabaseConnectionVM.GetConnectionString();
                var canConnect = await _dbConnector.TryConnect(_connectionString);
                if (canConnect)
                {
                    var tables = await _dbService.GetTables(_connectionString);
                    var hasTables = await TablesVM.SetTables(tables);
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
                else
                {
                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                    await DatabaseConnectionVM.RaiseCantConnect();
                    goBack();
                }
            }
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
            }
        }

        public void Dispose()
        {
            DatabaseConnectionVM.PropertyChanged -= onValidationStateChanged;
            TablesVM.PropertyChanged -= onValidationStateChanged;
            OutputParamsVM.PropertyChanged -= onValidationStateChanged;
        }
    }
}
