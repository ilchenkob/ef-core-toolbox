using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Toolbox.Extension.Logic.ViewModels.TreeNodes;

namespace Toolbox.Extension.Logic.Migrations.ViewModels.TreeNodes
{
    public class DbContextNodeViewModel : NodeViewModel
    {
        private bool _detectChanges = true;

        public DbContextNodeViewModel(string title, IEnumerable<string> migrations, Action<bool> stateChangedCallback)
        {
            Title = title;
            Childs = new ObservableCollection<NodeViewModel>(
                migrations.Select(t => new NodeViewModel(t, s => checkState())));
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
