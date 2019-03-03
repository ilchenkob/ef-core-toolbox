using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.SqlServer.Design.Internal;
using Microsoft.Extensions.DependencyInjection;
using Migrator.Logic.Models;
using System.IO;

namespace Migrator.Logic
{
    public class ScriptMigrationExecutor
    {
        public int Run(ScriptMigrationParams migrationParams)
        {
            if (!File.Exists(migrationParams.AssemblyFileName))
                return ExitCode.CanNotFindFile;

            var context = DbContextFactory.CreateContextInstance(migrationParams.AssemblyFileName, migrationParams.DbContextFullName);

            var serviceCollection = new ServiceCollection();
            serviceCollection
                .AddEntityFrameworkDesignTimeServices()
                .AddDbContextDesignTimeServices(context);

            var designTimeServices = new SqlServerDesignTimeServices();
            designTimeServices.ConfigureDesignTimeServices(serviceCollection);

            var serviceProvider = serviceCollection.BuildServiceProvider();
            var migrator = serviceProvider.GetService<IMigrator>();

            if (!Directory.Exists(migrationParams.OutputPath))
            {
                Directory.CreateDirectory(migrationParams.OutputPath);
            }

            foreach (var item in migrationParams.Migrations)
            {
                var migrationNames = item.Split(ScriptMigrationParams.MigrationNamesSplitter);
                var migrationSql = migrator.GenerateScript(
                                        fromMigration: migrationNames[0],
                                        toMigration: migrationNames[1],
                                        idempotent: true);

                File.WriteAllText(Path.Combine(migrationParams.OutputPath, $"{migrationNames[1]}.sql"), migrationSql);
            }

            return ExitCode.Success;
        }
    }
}
