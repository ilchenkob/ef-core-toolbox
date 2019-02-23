using System;
using System.ComponentModel.Design;
using System.Windows;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Toolbox.Extension.Logic.Scaffolding;
using Toolbox.Extension.Logic.Scaffolding.DatabaseServices;
using Toolbox.Extension.Logic.Scaffolding.ViewModels;
using Toolbox.Extension.UI.Scaffolding;
using Toolbox.Extension.UI.Services;
using Task = System.Threading.Tasks.Task;

namespace Toolbox.Extension
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class MenuCommand
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x0100;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("208610e3-fcba-47f3-b084-da779d2fb5e2");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly AsyncPackage package;

        private IServiceProvider serviceProvider => this.package;

        private readonly EnvDTE80.DTE2 _ide;

        /// <summary>
        /// Initializes a new instance of the <see cref="MenuCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="commandService">Command service to add command to, not null.</param>
        private MenuCommand(AsyncPackage package, OleMenuCommandService commandService)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new OleMenuCommand(this.MenuItemCallback, menuCommandID);
            commandService.AddCommand(menuItem);

            _ide = (EnvDTE80.DTE2)this.serviceProvider.GetService(typeof(DTE));
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static MenuCommand Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private Microsoft.VisualStudio.Shell.IAsyncServiceProvider ServiceProvider
        {
            get
            {
                return this.package;
            }
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static async Task InitializeAsync(AsyncPackage package)
        {
            // Switch to the main thread - the call to AddCommand in MenuCommand's constructor requires
            // the UI thread.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            OleMenuCommandService commandService = await package.GetServiceAsync((typeof(IMenuCommandService))) as OleMenuCommandService;
            Instance = new MenuCommand(package, commandService);
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void MenuItemCallback(object senderObj, EventArgs args)
        {
            if (senderObj is OleMenuCommand && args is OleMenuCmdEventArgs)
            {
                var solutionProcessor = new SolutionProcessor(_ide);
                var projects = solutionProcessor.GetAllSolutionProjects();

                var messageBoxService = new MessageBoxService();
                var dbService = new MsSqlServerService();
                var scaffoldingVM = new ScaffoldingWizardViewModel(
                    messageBoxService, dbService, dbService, new ScaffoldingService(), projects);
                var scaffoldingWindow = new ScaffoldingWizard(scaffoldingVM);

                messageBoxService.ShowInfoMessageFunc =
                    msg => showMessageBox(scaffoldingWindow, msg, MessageBoxImage.Information);
                messageBoxService.ShowErrorMessageFunc =
                    msg => showMessageBox(scaffoldingWindow, msg, MessageBoxImage.Error);
                messageBoxService.ShowWarningMessageFunc =
                    msg => showMessageBox(scaffoldingWindow, msg, MessageBoxImage.Warning);

                scaffoldingWindow.ShowModal();
            }
        }

        private async Task showMessageBox(ScaffoldingWizard owner, string text, MessageBoxImage icon)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            MessageBox.Show(owner, text, "Database Context Scaffolding", MessageBoxButton.OK, icon);
        }
    }
}
