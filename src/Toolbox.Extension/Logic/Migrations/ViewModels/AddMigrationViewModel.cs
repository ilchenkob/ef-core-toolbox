using Migrator.Logic.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Extension.Logic.ViewModels;

namespace Toolbox.Extension.Logic.Migrations.ViewModels
{
    public class AddMigrationViewModel : BaseViewModel
    {
        private readonly Dictionary<string, List<string>> _projectItems;
        private readonly IMigrationService _migrationService;

        public AddMigrationViewModel(Dictionary<string, List<string>> projectItems, IMigrationService migrationService)
        {
            _projectItems = projectItems;
            _migrationService = migrationService;

            ProjectNames = new ObservableCollection<string>(_projectItems.Keys.OrderBy(i => i));
            DbContextClassNames = new ObservableCollection<string>();

            SelectedProjectName = ProjectNames.FirstOrDefault();
            MigrationName = string.Empty;
        }

        public ObservableCollection<string> ProjectNames { get; private set; }

        private string _selectedProjectName;
        public string SelectedProjectName
        {
            get => _selectedProjectName;
            set
            {
                if (_selectedProjectName != value)
                {
                    _selectedProjectName = value;
                    DbContextClassNames.Clear();
                    foreach (var dbContextName in _projectItems[_selectedProjectName])
                        DbContextClassNames.Add(dbContextName);

                    NotifyPropertyChanged(() => SelectedProjectName);
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
                    NotifyPropertyChanged(() => SelectedDbContextName);
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
                }
            }
        }
    }
}
