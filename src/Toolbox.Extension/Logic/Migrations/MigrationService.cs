using Migrator.Logic.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
                return output.Split(';').ToList();
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
