using System;
using System.Collections.ObjectModel;

namespace Toolbox.Extension.Logic.ViewModels.TreeNodes
{
    public class NodeViewModel : BaseViewModel
    {
        public NodeViewModel()
        {
            Title = string.Empty;
            Childs = new ObservableCollection<NodeViewModel>();
            IsEnabled = false;
        }

        public NodeViewModel(string title, Action<bool> stateChangedCallback) : this()
        {
            Title = title;
            StateChanged = stateChangedCallback;
        }

        public NodeViewModel(string title, string subtitle, Action<bool> stateChangedCallback) : this(title, stateChangedCallback)
        {
            Subtitle = subtitle;
        }

        public Action<bool> StateChanged { get; set; }

        private bool _isEnabled;
        public bool IsEnabled
        {
            get => _isEnabled;
            private set
            {
                _isEnabled = value;
                NotifyPropertyChanged(() => IsEnabled);
            }
        }

        private string _title;
        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                NotifyPropertyChanged(() => Title);
            }
        }

        private string _subtitle;
        public string Subtitle
        {
            get => _subtitle;
            set
            {
                _subtitle = value;
                NotifyPropertyChanged(() => Subtitle);
            }
        }

        public ObservableCollection<NodeViewModel> Childs { get; protected set; }

        public virtual void ChangeState(bool isEnabled)
        {
            IsEnabled = isEnabled;
            StateChanged?.Invoke(IsEnabled);
        }
    }
}
