using System.Windows;
using System.Windows.Input;
using Toolbox.Extension.Logic.Scaffolding.ViewModels;

namespace Toolbox.Extension.UI.Scaffolding
{
    /// <summary>
    /// Interaction logic for ScaffoldingWizard.xaml
    /// </summary>
    public partial class ScaffoldingWizard
    {
        private readonly ScaffoldingWizardViewModel _viewModel;

        public static Window Instance { get; private set; }

        public ScaffoldingWizard(ScaffoldingWizardViewModel viewModel)
        {
            InitializeComponent();
            Instance = this;

            _viewModel = viewModel;
            _viewModel.CloseAction = Close;

            DataContext = viewModel;

            KeyDown += OnKeyDown;
            Closing += CloseHandler;
        }

        private void CloseHandler(object sender, System.ComponentModel.CancelEventArgs e)
        {
            KeyDown -= OnKeyDown;
            Closing -= CloseHandler;

            _viewModel?.Dispose();
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
