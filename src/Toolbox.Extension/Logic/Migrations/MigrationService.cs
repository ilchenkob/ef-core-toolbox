using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Design.Internal;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Design;
using Microsoft.EntityFrameworkCore.SqlServer.Design.Internal;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Toolbox.Extension.Logic.Migrations
{
    internal class MigrationService
    {
        public int Run()
        {
            try
            {
                // var assemblyFileName = "";
                // var assembly = Assembly.LoadFile(assemblyFileName);
                DbContext context = new DbCtx();
                ICurrentDbContext currentDbContext = new CurrentDbContext(context);

                var serviceCollection = new ServiceCollection();

                serviceCollection
                    .AddEntityFrameworkDesignTimeServices()
                    .AddTransient(provider => currentDbContext);

                var designTimeServices = new SqlServerDesignTimeServices();
                designTimeServices.ConfigureDesignTimeServices(serviceCollection);

                var serviceProvider = serviceCollection.BuildServiceProvider();

                var migrationScaffolder = serviceProvider.GetService<IMigrationsScaffolder>();
                
                var migration = migrationScaffolder.ScaffoldMigration("", "", "");
                migrationScaffolder.Save("", migration, "");
                
            }
            catch (System.Exception ex)
            {
                var a = ex.Message;
            }


            return 1;
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

    internal class DbCtx : DbContext
    {

    }
}
