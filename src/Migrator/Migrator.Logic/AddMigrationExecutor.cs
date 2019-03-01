using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Migrations.Design;
using Microsoft.EntityFrameworkCore.SqlServer.Design.Internal;
using Microsoft.Extensions.DependencyInjection;
using Migrator.Logic.Models;
using System.IO;

namespace Migrator.Logic
{
    public class AddMigrationExecutor
    {
        public int Run(AddMigrationParams migrationParams)
        {
            if (!File.Exists(migrationParams.AssemblyFileName))
                return ExitCode.CanNotFindFile;

            var context = DbContextFactory.CreateContextInstance(migrationParams.AssemblyFileName, migrationParams.DbContextTypeFullName);

            var serviceCollection = new ServiceCollection();
            serviceCollection
                .AddEntityFrameworkDesignTimeServices()
                .AddDbContextDesignTimeServices(context);

            var designTimeServices = new SqlServerDesignTimeServices();
            designTimeServices.ConfigureDesignTimeServices(serviceCollection);

            var serviceProvider = serviceCollection.BuildServiceProvider();
            var migrationScaffolder = serviceProvider.GetService<IMigrationsScaffolder>();

            var migration = migrationScaffolder.ScaffoldMigration(
                migrationName: migrationParams.MigrationName, 
                rootNamespace: migrationParams.ContextNamespace,
                subNamespace: migrationParams.SubNamespace,
                language: Constants.ProgrammingLanguage);
            var files = migrationScaffolder.Save(migrationParams.ProjectDir, migration, migrationParams.OutputDir);

            return ExitCode.Success;
        }
    }
}
