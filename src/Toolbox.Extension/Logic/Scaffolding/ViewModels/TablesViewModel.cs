﻿using Microsoft.VisualStudio.Shell;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Toolbox.Extension.Logic.Scaffolding.ViewModels.TreeNodes;
using Toolbox.Extension.UI.Services;

namespace Toolbox.Extension.Logic.Scaffolding.ViewModels
{
    public class TablesViewModel : BaseViewModel
    {
        private readonly IMessageBoxService _messageBoxService;
        public TablesViewModel(IMessageBoxService messageBoxService)
        {
            _messageBoxService = messageBoxService;

            Schemas = new ObservableCollection<NodeViewModel>();
        }

        public bool IsValid => Schemas.Any(s => s.IsEnabled);

        public ObservableCollection<NodeViewModel> Schemas { get; private set; }

        public async Task<bool> SetTables(Dictionary<string, List<string>> schemas)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            Schemas.Clear();

            if (schemas.Keys.Count == 0)
            {
                await _messageBoxService.ShowWarningMessage("There is not tables in database");
                return false;
            }

            foreach (var schema in schemas.OrderBy(s => s.Key))
            {
                var node = new SchemaNodeViewModel(
                    schema.Key,
                    schema.Value,
                    s => NotifyPropertyChanged(() => IsValid));
                Schemas.Add(node);
            }

            NotifyPropertyChanged(() => Schemas);
            NotifyPropertyChanged(() => IsValid);

            return true;
        }
        
        public List<string> GetSelectedTables()
        {
            return Schemas.Where(s => s.IsEnabled)
                          .SelectMany(s => s.Childs
                                            .Where(t => t.IsEnabled)
                                            .Select(t => $"{s.Title}.{t.Title}")
                                     )
                          .ToList();
        }
    }
}
