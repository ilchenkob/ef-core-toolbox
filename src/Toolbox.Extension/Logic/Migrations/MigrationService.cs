using Migrator.Logic.Models;
using System.Collections.Generic;
using System.Linq;
using Toolbox.Extension.UI.Services;

namespace Toolbox.Extension.Logic.Migrations
{
    internal class MigrationService : IMigrationService
    {
        private readonly IMessageBoxService _messageBoxService;

        public MigrationService(IMessageBoxService messageBoxService)
        {
            _messageBoxService = messageBoxService;
        }

        public int AddMigration(AddMigrationParams commandParams)
        {
            var processRunner = new ProcessRunner();
            return processRunner.Execute(commandParams);
        }

        public int ScriptMigration(ScriptMigrationParams commandParams)
        {
            var processRunner = new ProcessRunner();
            return processRunner.Execute(commandParams);
        }

        public List<string> GetDbContextNames(FindDbContextSubtypeParams commandParams)
        {
            var result = runMigratorProcess(commandParams);
            if (!string.IsNullOrEmpty(result.error))
            {
#pragma warning disable VSTHRD110 // Observe result of async calls
                _messageBoxService.ShowErrorMessage(result.error);
#pragma warning restore VSTHRD110 // Observe result of async calls
            }

            if (result.exitCode == ExitCode.Success)
            {
                if (!string.IsNullOrWhiteSpace(result.output))
                    return result.output.Split(';').ToList();
            }
            else if (result.exitCode == ExitCode.Exception && !string.IsNullOrWhiteSpace(result.output))
            {
#pragma warning disable VSTHRD110 // Observe result of async calls
                _messageBoxService.ShowErrorMessage(result.output);
#pragma warning restore VSTHRD110 // Observe result of async calls
            }

            return new List<string>();
        }

        public Dictionary<string, List<string>> GetMigrationNames(FindMigrationSubtypeParams commandParams)
        {
            var items = new Dictionary<string, List<string>>();

            var result = runMigratorProcess(commandParams);
            if (!string.IsNullOrEmpty(result.error))
            {
#pragma warning disable VSTHRD110 // Observe result of async calls
                _messageBoxService.ShowErrorMessage(result.error);
#pragma warning restore VSTHRD110 // Observe result of async calls
            }

            if (result.exitCode == ExitCode.Success && !string.IsNullOrWhiteSpace(result.output))
            {
                var groups = result.output.Split('|').Where(n => !string.IsNullOrEmpty(n));
                foreach (var group in groups)
                {
                    var names = group.Split(';').Where(n => !string.IsNullOrEmpty(n));
                    if (names != null && names.Count() > 1)
                    {
                        items[names.First()] = names.Skip(1).ToList();
                    }
                }
            }
            else if (result.exitCode == ExitCode.Exception && !string.IsNullOrWhiteSpace(result.output))
            {
#pragma warning disable VSTHRD110 // Observe result of async calls
                _messageBoxService.ShowErrorMessage(result.output);
#pragma warning restore VSTHRD110 // Observe result of async calls
            }

            return items;
        }

        private (int exitCode, string output, string error) runMigratorProcess(IMigratorParams commandParams)
        {
            var output = string.Empty;
            var error = string.Empty;
            var result = new ProcessRunner
            {
                OutputDataCallback = (p, msg) => output = msg,
                ErrorDataCallback = (p, msg) => error = msg
            }
            .Execute(commandParams);

            return (result, output, error);
        }
    }
}
