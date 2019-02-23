using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Toolbox.Extension.Logic.Scaffolding.ViewModels.TreeNodes
{
    public class SchemaNodeViewModel : NodeViewModel
    {
        private bool _detectChanges = true;

        public SchemaNodeViewModel(string title, IEnumerable<string> tables, Action<bool> stateChangedCallback)
        {
            Title = title;
            Childs = new ObservableCollection<NodeViewModel>(
                tables.Select(t => new NodeViewModel(t, s => checkState())));
            StateChanged = stateChangedCallback;
        }

        public override void ChangeState(bool isEnabled)
        {
            if (_detectChanges)
            {
                base.ChangeState(isEnabled);
                _detectChanges = false;
                foreach (var prop in Childs)
                {
                    prop.ChangeState(IsEnabled);
                }
                _detectChanges = true;
            }
        }

        private void checkState()
        {
            if (_detectChanges)
            {
                _detectChanges = false;
                base.ChangeState(Childs.Any(c => c.IsEnabled));
                _detectChanges = true;
            }
        }
    }
}