using Microsoft.VisualStudio.Shell;
using Migrator.Logic.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using Toolbox.Extension.Logic.Migrations.ViewModels.TreeNodes;
using Toolbox.Extension.Logic.Models;
using Toolbox.Extension.Logic.Scaffolding.ViewModels.TreeNodes;
using Toolbox.Extension.Logic.ViewModels;
using Toolbox.Extension.Logic.ViewModels.TreeNodes;
using Toolbox.Extension.UI.Services;
using Task = System.Threading.Tasks.Task;

namespace Toolbox.Extension.Logic.Migrations.ViewModels
{
    public class ScriptMigrationViewModel : BaseViewModel
    {
        private readonly IReadOnlyCollection<Project> _allSolutionProjects;
        private readonly Dictionary<string, Dictionary<string, List<string>>> _projectMigrations;

        private readonly IProjectBuilder _projectBuilder;
        private readonly IMigrationService _migrationService;
        private readonly IMessageBoxService _messageBoxService;

        public ScriptMigrationViewModel(
            IReadOnlyCollection<Project> allSolutionProjects,
            IProjectBuilder projectBuilder,
            IMigrationService migrationService,
            IMessageBoxService messageBoxService)
        {
            _allSolutionProjects = allSolutionProjects;
            _projectBuilder = projectBuilder;
            _migrationService = migrationService;
            _messageBoxService = messageBoxService;

            ProjectNames = new ObservableCollection<string>(allSolutionProjects.Select(p => p.DisplayName));
            Migrations = new ObservableCollection<DbContextNodeViewModel>();
            _projectMigrations = new Dictionary<string, Dictionary<string, List<string>>>();

            SelectedProjectName = ProjectNames.FirstOrDefault();

            OkCommand = new AsyncCommand(okCommandExecute);
        }

        public ICommand OkCommand { get; private set; }

        public ObservableCollection<string> ProjectNames { get; private set; }

        private string _selectedProjectName;
        public string SelectedProjectName
        {
            get => _selectedProjectName;
            set
            {
                if (!string.IsNullOrWhiteSpace(value) && _selectedProjectName != value)
                {
                    _selectedProjectName = value;

                    OutputPath = Path.Combine(
                        _allSolutionProjects.FirstOrDefault(p => p.DisplayName == _selectedProjectName)?.Path,
                        Constants.DefaultMigrationSubDirectory);

                    loadMigrationNames(value);
                    NotifyPropertyChanged(() => SelectedProjectName);
                    NotifyPropertyChanged(() => IsValid);
                }
            }
        }

        public ObservableCollection<DbContextNodeViewModel> Migrations { get; private set; }

        private string _outputPath;
        public string OutputPath
        {
            get => _outputPath;
            set
            {
                _outputPath = value;
                NotifyPropertyChanged(() => OutputPath);
                NotifyPropertyChanged(() => IsValid);
            }
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;
                    NotifyPropertyChanged(() => IsLoading);
                }
            }
        }

        public bool IsValid
        {
            get => Migrations.Any(m => m.IsEnabled) &&
                !string.IsNullOrWhiteSpace(SelectedProjectName) &&
                !string.IsNullOrWhiteSpace(OutputPath);
        }

        private async Task okCommandExecute()
        {
            var selectedProject = _allSolutionProjects.FirstOrDefault(p => p.DisplayName == SelectedProjectName);
            if (selectedProject != null)
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                IsLoading = true;
                await Task.Run(async () =>
                {
                    var selectedDbContext = Migrations.First(r => r.IsEnabled);
                    var exitCode = _migrationService.ScriptMigration(new ScriptMigrationParams
                    {
                        AssemblyFileName = selectedProject.AssemblyNameWithPath,
                        DbContextFullName = selectedDbContext.Title,
                        Migrations = getSelectedMigrationsList(selectedDbContext.Childs).ToArray(),
                        OutputPath = this.OutputPath
                    });
                    if (exitCode == ExitCode.Success)
                    {
                        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                        CloseAction?.Invoke();
                    }
                });
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                IsLoading = false;
            }
        }

        private void loadMigrationNames(string projectName)
        {
            if (!_projectMigrations.ContainsKey(projectName))
            {
#pragma warning disable VSTHRD110 // Observe result of async calls
                Task.Run(async () =>
#pragma warning restore VSTHRD110 // Observe result of async calls
                {
                    var selectedProject = _allSolutionProjects.FirstOrDefault(p => p.DisplayName == projectName);
                    if (selectedProject == null)
                        return;

                    var contextMigrations = _migrationService.GetMigrationNames(new FindMigrationSubtypeParams
                    {
                        AssemblyFileName = selectedProject.AssemblyNameWithPath
                    });
                    _projectMigrations[projectName] = contextMigrations;

                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                    refreshMigrationsCollection(projectName);
                });
            }
            else
            {
                refreshMigrationsCollection(projectName);
            }
        }

        private void refreshMigrationsCollection(string projectName)
        {
            Migrations.Clear();
            foreach (var item in _projectMigrations[projectName])
            {
                Migrations.Add(new DbContextNodeViewModel(
                    item.Key,
                    item.Value.Select(m => m.Insert(m.IndexOf("_"),": ")).OrderBy(m => m),
                    isChecked =>
                    {
                        NotifyPropertyChanged(() => IsValid);
                    }));
            }
        }

        private List<string> getSelectedMigrationsList(IEnumerable<NodeViewModel> migrations)
        {
            var result = new List<string>();
            var migrationsCount = migrations.Count();
            for (int i=0; i<migrationsCount; i++)
            {
                var currItem = migrations.ElementAt(i);
                if (currItem.IsEnabled)
                {
                    var fromMigration = i == 0 ? string.Empty : migrations.ElementAt(i -1).Title.Replace(": ", "");
                    var toMigration = currItem.Title.Replace(": ", "");
                    result.Add($"{fromMigration}{ScriptMigrationParams.MigrationNamesSplitter}{toMigration}");
                }
            }
            return result;
        }
    }
}
