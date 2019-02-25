using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Design.Internal;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.SqlServer.Design.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace Toolbox.Extension.Logic.Migrations
{
    internal class MigrationService
    {
        //public int Run()
        //{
        //    var serviceCollection = new ServiceCollection();

        //    serviceCollection
        //        .AddEntityFrameworkDesignTimeServices()
        //        .AddSingleton<IOperationReporter, OperationReporter>()
        //        .AddSingleton<IOperationReportHandler, OperationReportHandler>();

        //    //if (scaffoldingParams.UseInflector)
        //    //{
        //    //    serviceCollection.AddSingleton<IPluralizer, Pluralizer>();
        //    //}

        //    var provider = new SqlServerDesignTimeServices();
        //    provider.ConfigureDesignTimeServices(serviceCollection);
        //    //if (!string.IsNullOrEmpty(reverseEngineerOptions.Dacpac))
        //    //{
        //    //    serviceCollection.AddSingleton<IDatabaseModelFactory, SqlServerDacpacDatabaseModelFactory>();
        //    //}

        //    var serviceProvider = serviceCollection.BuildServiceProvider();
        //    var sqlGenerator = serviceProvider.GetService<IMigrationsSqlGenerator>();

        //    return 1;
        //}

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
