using Microsoft.VisualStudio.Shell;
using Migrator.Logic.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using Toolbox.Extension.Logic.Models;
using Toolbox.Extension.Logic.ViewModels;
using Toolbox.Extension.UI.Services;
using Task = System.Threading.Tasks.Task;

namespace Toolbox.Extension.Logic.Migrations.ViewModels
{
    public class AddMigrationViewModel : BaseViewModel
    {
        private readonly IReadOnlyCollection<Project> _allSolutionProjects;
        private readonly Dictionary<string, List<DbContextInfo>> _projectDbContexts;

        private readonly IProjectBuilder _projectBuilder;
        private readonly IMigrationService _migrationService;
        private readonly IMessageBoxService _messageBoxService;

        public AddMigrationViewModel(
            IReadOnlyCollection<Project> allSolutionProjects,
            IProjectBuilder projectBuilder,
            IMigrationService migrationService,
            IMessageBoxService messageBoxService)
        {
            _allSolutionProjects = allSolutionProjects;
            _projectBuilder = projectBuilder;
            _migrationService = migrationService;
            _messageBoxService = messageBoxService;

            _projectDbContexts = new Dictionary<string, List<DbContextInfo>>();

            ProjectNames = new ObservableCollection<string>(allSolutionProjects.Select(p => p.DisplayName));
            DbContextClassNames = new ObservableCollection<string>();

            SelectedProjectName = ProjectNames.FirstOrDefault();
            MigrationNamespace =
            MigrationName = string.Empty;

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

                    loadDbContextNames(value);
                    NotifyPropertyChanged(() => SelectedProjectName);
                    NotifyPropertyChanged(() => IsValid);
                }
            }
        }

        public ObservableCollection<string> DbContextClassNames { get; private set; }

        private string _selectedDbContextName;
        public string SelectedDbContextName
        {
            get => _selectedDbContextName;
            set
            {
                if (_selectedDbContextName != value)
                {
                    _selectedDbContextName = value;
                    if (_selectedDbContextName != null)
                    {
                        var contextNamespace = _projectDbContexts[SelectedProjectName].FirstOrDefault(n => n.ClassName == value).Namespace ?? string.Empty;
                        MigrationNamespace = contextNamespace.Length > 0
                            ? $"{contextNamespace}.{Constants.DefaultMigrationSubNamespace}"
                            : Constants.DefaultMigrationSubNamespace;
                    }

                    NotifyPropertyChanged(() => SelectedDbContextName);
                    NotifyPropertyChanged(() => IsValid);
                }
            }
        }

        private string _migrationName;
        public string MigrationName
        {
            get => _migrationName;
            set
            {
                if (_migrationName != value)
                {
                    _migrationName = value;
                    NotifyPropertyChanged(() => MigrationName);
                    NotifyPropertyChanged(() => IsValid);
                }
            }
        }

        private string _migrationNamespace;
        public string MigrationNamespace
        {
            get => _migrationNamespace;
            set
            {
                if (_migrationNamespace != value)
                {
                    _migrationNamespace = value;
                    NotifyPropertyChanged(() => MigrationNamespace);
                    NotifyPropertyChanged(() => IsValid);
                }
            }
        }

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
            get =>
                !string.IsNullOrWhiteSpace(SelectedProjectName) &&
                !string.IsNullOrWhiteSpace(SelectedDbContextName) &&
                !string.IsNullOrWhiteSpace(MigrationNamespace) &&
                !string.IsNullOrWhiteSpace(MigrationName) &&
                !string.IsNullOrWhiteSpace(OutputPath);
        }

        private async Task okCommandExecute()
        {
            var selectedProject = _allSolutionProjects.FirstOrDefault(p => p.DisplayName == SelectedProjectName);
            if (selectedProject != null)
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                IsLoading = true;
                if (_projectBuilder.Build(selectedProject.UniqueName))
                {
                    await Task.Run(async () =>
                    {
                        var selectedContext = _projectDbContexts[selectedProject.DisplayName]
                                                .FirstOrDefault(c => c.ClassName == SelectedDbContextName);
                        var commandParams = new AddMigrationParams
                        {
                            AssemblyFileName = selectedProject.AssemblyNameWithPath,
                            MigrationName = MigrationName,
                            ProjectDir = selectedProject.Path,
                            DbContextTypeFullName = selectedContext.FullName,
                            ContextNamespace = selectedContext.Namespace,
                            SubNamespace = getSubNamespace(selectedContext.Namespace, MigrationNamespace),
                            OutputDir = OutputPath
                        };
                        var commandResult = _migrationService.AddMigration(commandParams);
                        if (commandResult == ExitCode.Success)
                        {
                            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                            CloseAction?.Invoke();
                        }
                        else
                        {
                            // TODO: show message
                        }
                    });
                }
                else
                {
                    await _messageBoxService.ShowErrorMessage(
                        string.Format(Resources.Strings.MessageCantBuildProject, selectedProject.DisplayName));
                }

                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                IsLoading = false;
            }
        }

        private void loadDbContextNames(string projectName)
        {
            if (!_projectDbContexts.ContainsKey(projectName))
            {
                IsLoading = true;
#pragma warning disable VSTHRD110 // Observe result of async calls
                Task.Run(async () =>
#pragma warning restore VSTHRD110 // Observe result of async calls
                {
                    var selectedProject = _allSolutionProjects.FirstOrDefault(p => p.DisplayName == projectName);
                    if (selectedProject == null)
                    {
                        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                        IsLoading = false;
                        return;
                    }

                    var classNames = _migrationService.GetDbContextNames(new FindDbContextSubtypeParams
                    {
                        AssemblyFileName = selectedProject.AssemblyNameWithPath
                    });
                    _projectDbContexts[projectName] = classNames.Select(n =>
                    {
                        var lastDotIndex = n.LastIndexOf('.');
                        return new DbContextInfo
                        {
                            FullName = n,
                            Namespace = n.Substring(0, lastDotIndex),
                            ClassName = n.Substring(lastDotIndex + 1)
                        };
                    }).OrderBy(d => d.ClassName).ToList();

                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                    refreshDbContextNamesCollection(projectName);
                    IsLoading = false;
                });
            }
            else
            {
                refreshDbContextNamesCollection(projectName);
            }
        }

        private void refreshDbContextNamesCollection(string projectName)
        {
            DbContextClassNames.Clear();
            foreach (var dbContextName in _projectDbContexts[projectName])
            {
                DbContextClassNames.Add(dbContextName.ClassName);
            }
            SelectedDbContextName = DbContextClassNames.FirstOrDefault();
        }

        private string getSubNamespace(string rootNamespace, string namespaceValue)
        {
            var result = namespaceValue.StartsWith(rootNamespace)
                ? namespaceValue.Substring(rootNamespace.Length + 1)
                : namespaceValue;

            return result.TrimStart('.', ' ');
        }

        private struct DbContextInfo
        {
            public string FullName { get; set; }

            public string ClassName { get; set; }

            public string Namespace { get; set; }
        }
    }
}
