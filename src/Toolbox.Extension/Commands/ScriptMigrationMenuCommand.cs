using EnvDTE;
using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel.Design;
using System.Windows;
using Toolbox.Extension.Logic.Migrations;
using Toolbox.Extension.Logic.Migrations.ViewModels;
using Toolbox.Extension.Resources;
using Toolbox.Extension.UI.Migrations;
using Toolbox.Extension.UI.Services;
using Task = System.Threading.Tasks.Task;

namespace Toolbox.Extension.Commands
{
    internal sealed class ScriptMigrationMenuCommand : IDisposable
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int ScriptMigrationgCommandId = 0x0300;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("208610e3-fcba-47f3-b084-da779d2fb5e2");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly AsyncPackage package;

        private readonly EnvDTE80.DTE2 _ide;

        private readonly OleMenuCommand _scaffoldingMenuItem;

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private IAsyncServiceProvider serviceProvider => package;

        /// <summary>
        /// Initializes a new instance of the <see cref="MenuCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="commandService">Command service to add command to, not null.</param>
        private ScriptMigrationMenuCommand(AsyncPackage package, OleMenuCommandService commandService, EnvDTE80.DTE2 ide)
        {
            if (commandService == null) throw new ArgumentNullException(nameof(commandService));

            this.package = package ?? throw new ArgumentNullException(nameof(package));
            _ide = ide ?? throw new ArgumentNullException(nameof(ide));

            var scaffoldingCommandID = new CommandID(CommandSet, ScriptMigrationgCommandId);
            _scaffoldingMenuItem = new OleMenuCommand(MenuItemCallback, scaffoldingCommandID);
            _scaffoldingMenuItem.BeforeQueryStatus += QueryCommandStatus;

            commandService.AddCommand(_scaffoldingMenuItem);
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static ScriptMigrationMenuCommand Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static async Task InitializeAsync(AsyncPackage package)
        {
            var ide = await package.GetServiceAsync(typeof(DTE)) as EnvDTE80.DTE2;
            var commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;

            // Switch to the main thread - the call to AddCommand in MenuCommand's constructor requires
            // the UI thread.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);
            Instance = new ScriptMigrationMenuCommand(package, commandService, ide);
        }

        private void QueryCommandStatus(object sender, EventArgs e)
        {
            if (sender is OleMenuCommand menuCommand && menuCommand.CommandID.ID == ScriptMigrationgCommandId)
            {
                menuCommand.Enabled = isMigrationCommandEnabled();
            }
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
                var migrationService = new MigrationService(messageBoxService);

                var viewModel = new ScriptMigrationViewModel(projects, solutionProcessor, migrationService, messageBoxService);
                var window = new ScriptMigration(viewModel);

                messageBoxService.ShowInfoMessageFunc =
                    msg => showMessageBox(window, msg, MessageBoxImage.Information);
                messageBoxService.ShowErrorMessageFunc =
                    msg => showMessageBox(window, msg, MessageBoxImage.Error);
                messageBoxService.ShowWarningMessageFunc =
                    msg => showMessageBox(window, msg, MessageBoxImage.Warning);

                window.ShowModal();
            }
        }

        private async Task showMessageBox(ScriptMigration owner, string text, MessageBoxImage icon)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            MessageBox.Show(owner, text, Strings.ScriptMigrationMenuItemTitle, MessageBoxButton.OK, icon);
        }

        private bool isMigrationCommandEnabled()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (_ide?.Solution != null)
            {
                return _ide.Solution.IsOpen &&
                    // _ide.Solution.SolutionBuild.BuildState == vsBuildState.vsBuildStateDone;
                    _ide.Solution.SolutionBuild.BuildState != vsBuildState.vsBuildStateInProgress;
            }

            return false;
        }

        public void Dispose()
        {
            if (_scaffoldingMenuItem != null)
                _scaffoldingMenuItem.BeforeQueryStatus -= QueryCommandStatus;
        }
    }
}
