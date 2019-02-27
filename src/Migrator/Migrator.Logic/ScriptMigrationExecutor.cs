using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Migrations;
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
    public class ScriptMigrationExecutor
    {
        public int Run(ScriptMigrationParams migrationParams)
        {
            if (!File.Exists(migrationParams.AssemblyFileName))
                return ExitCode.CanNotFindFile;

            var assembly = Assembly.LoadFile(migrationParams.AssemblyFileName);
            Type contextType = null;// assembly.GetExportedTypes().FirstOrDefault(t => t.FullName == migrationParams.DbContextTypeFullName);
            var context = (DbContext)Activator.CreateInstance(contextType);

            var serviceCollection = new ServiceCollection();

            serviceCollection
                .AddEntityFrameworkDesignTimeServices()
                .AddDbContextDesignTimeServices(context);

            var designTimeServices = new SqlServerDesignTimeServices();
            designTimeServices.ConfigureDesignTimeServices(serviceCollection);

            var serviceProvider = serviceCollection.BuildServiceProvider();
            var sqlGenerator = serviceProvider.GetService<IMigrationsSqlGenerator>();

            // sqlGenerator.Generate();

            return ExitCode.Success;
        }
    }
}
