using Migrator.Logic.Models;
using System.Collections.Generic;
using System.Linq;
using Toolbox.Extension.UI.Services;

namespace Toolbox.Extension.Logic.Migrations
{
    public interface IMigrationService
    {
        int AddMigration(AddMigrationParams addMigratorParams);

        List<string> GetDbContextNames(FindDbContextSubtypeParams commandParams);
    }

    internal class MigrationService : IMigrationService
    {
        private readonly IMessageBoxService _messageBoxService;

        public MigrationService(IMessageBoxService messageBoxService)
        {
            _messageBoxService = messageBoxService;
        }

        public int AddMigration(AddMigrationParams addMigratorParams)
        {
            var processRunner = new ProcessRunner();
            return processRunner.Execute(addMigratorParams);
        }

        public List<string> GetDbContextNames(FindDbContextSubtypeParams commandParams)
        {
            var output = string.Empty;
            var result = new ProcessRunner
            {
                OutputDataCallback = (p, msg) => output = msg
            }.Execute(commandParams);
            if (result == ExitCode.Success)
            {
                return string.IsNullOrWhiteSpace(output)
                    ? new List<string> { Resources.Strings.DbContextNotFoundComboBoxItem }
                    : output.Split(';').ToList();
            }
            else if (result == ExitCode.CanNotFindDbContext)
            {
                return new List<string> { Resources.Strings.DbContextNotFoundComboBoxItem };
            }
            else if (result == ExitCode.CanNotFindFile)
            {
                return new List<string> { Resources.Strings.CantBuildProjectComboBoxItem };
            }
            else if (!string.IsNullOrWhiteSpace(output))
            {
#pragma warning disable VSTHRD110 // Observe result of async calls
                _messageBoxService.ShowErrorMessage(output);
#pragma warning restore VSTHRD110 // Observe result of async calls
            }

            return new List<string>();
        }

        //public static string GenerateCreateScript(this DatabaseFacade database)
        //{
        //    var model = database.GetService<IModel>();
        //    var differ = database.GetService<IMigrationsModelDiffer>();
        //    var generator = database.GetService<IMigrationsSqlGenerator>();
        //    var sql = database.GetService<ISqlGenerationHelper>();

        //    var operations = differ.GetDifferences(null, model);
        //    var commands = generator.Generate(operations, model);

        //    var builder = new StringBuilder();
        //    foreach (var command in commands)
        //    {
        //        builder
        //            .Append(command.CommandText)
        //            .AppendLine(sql.BatchTerminator);
        //    }

        //    return builder.ToString();
        //}
    }
}
