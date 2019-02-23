using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Toolbox.Extension.Logic.Models;

namespace Toolbox.Extension.Logic.Scaffolding.ViewModels
{
    public class OutputParamsViewModel : BaseViewModel
    {
        private const string DefaultDbContextClassName = "DbContext"
;
        private readonly IReadOnlyCollection<Project> _allSolutionProjects;

        public OutputParamsViewModel(
            IReadOnlyCollection<Project> allSolutionProjects)
        {
            _allSolutionProjects = allSolutionProjects;

            ProjectNames = new ObservableCollection<string>(allSolutionProjects.Select(p => p.DisplayName));
            SelectedProjectName = ProjectNames.FirstOrDefault();
            ContextName = DefaultDbContextClassName;
            UseDataAnnotations = false;
            UseDbNames = false;
        }

        public bool IsValid =>
            _selectedProjectItem != null &&
            !string.IsNullOrWhiteSpace(SelectedProjectName) &&
            !string.IsNullOrWhiteSpace(Namespace) &&
            !string.IsNullOrWhiteSpace(ContextName);

        public ObservableCollection<string> ProjectNames { get; private set; }

        private Project _selectedProjectItem;
        private string _selectedProjectName;
        public string SelectedProjectName
        {
            get => _selectedProjectName;
            set
            {
                if (_selectedProjectItem != null)
                    _selectedProjectItem.IsSelected = false;

                _selectedProjectName = value;
                _selectedProjectItem = _allSolutionProjects.FirstOrDefault(p => p.DisplayName == value);
                _selectedProjectItem.IsSelected = true;

                Namespace = _selectedProjectItem.DefaultNamespace;
                OutputPath = _selectedProjectItem.Path;
                NotifyPropertyChanged(() => SelectedProjectName);
            }
        }

        private string _contextName;
        public string ContextName
        {
            get => _contextName;
            set
            {
                _contextName = value;
                NotifyPropertyChanged(() => ContextName);
            }
        }

        private string _namespace;
        public string Namespace
        {
            get => _namespace;
            set
            {
                _namespace = value;
                NotifyPropertyChanged(() => Namespace);
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
            }
        }

        private bool _useDbNames;
        public bool UseDbNames
        {
            get => _useDbNames;
            set
            {
                _useDbNames = value;
                NotifyPropertyChanged(() => UseDbNames);
            }
        }

        private bool _useDataAnnotations;
        public bool UseDataAnnotations
        {
            get => _useDataAnnotations;
            set
            {
                _useDataAnnotations = value;
                NotifyPropertyChanged(() => UseDataAnnotations);
            }
        }
    }
}
