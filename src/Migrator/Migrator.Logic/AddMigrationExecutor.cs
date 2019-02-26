using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Migrations.Design;
using Microsoft.EntityFrameworkCore.SqlServer.Design.Internal;
using Microsoft.Extensions.DependencyInjection;
using Migrator.Logic.Models;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Migrator.Logic
{
    public class AddMigrationExecutor : IAddMigrationExecutor
    {
        public int Run(AddMigratorParams migrationParams)
        {
            if (!File.Exists(migrationParams.AssemblyFileName))
                return ExitCode.CanNotFindFile;

            try
            {
                //var assemblyFileName = @"C:\Users\Vitalii_Ilchenko\source\repos\ConsoleApp9\ConsoleAppcore\bin\Debug\netcoreapp2.1\ConsoleAppcore.dll";
                var assembly = Assembly.LoadFile(migrationParams.AssemblyFileName);
                var contextType = assembly.GetExportedTypes().FirstOrDefault(t => t.FullName == migrationParams.DbContextTypeFullName);
                var context = (DbContext)Activator.CreateInstance(contextType);

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
            }
            catch (System.Exception ex)
            {
                return ExitCode.Exception;
            }

            return ExitCode.Success;
        }
    }
}
