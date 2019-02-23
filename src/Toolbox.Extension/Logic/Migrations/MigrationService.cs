using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Design.Internal;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.SqlServer.Design.Internal;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolbox.Extension.Logic.Migrations
{
    internal class MigrationService
    {
        public int Run()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection
                .AddEntityFrameworkDesignTimeServices()
                .AddSingleton<IOperationReporter, OperationReporter>()
                .AddSingleton<IOperationReportHandler, OperationReportHandler>();

            //if (scaffoldingParams.UseInflector)
            //{
            //    serviceCollection.AddSingleton<IPluralizer, Pluralizer>();
            //}

            var provider = new SqlServerDesignTimeServices();
            provider.ConfigureDesignTimeServices(serviceCollection);
            //if (!string.IsNullOrEmpty(reverseEngineerOptions.Dacpac))
            //{
            //    serviceCollection.AddSingleton<IDatabaseModelFactory, SqlServerDacpacDatabaseModelFactory>();
            //}

            var serviceProvider = serviceCollection.BuildServiceProvider();
            var sqlGenerator = serviceProvider.GetService<IMigrationsSqlGenerator>();

            return 1;
        }
    }
}
