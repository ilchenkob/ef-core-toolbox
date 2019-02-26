using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Design.Internal;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Design;
using Microsoft.EntityFrameworkCore.SqlServer.Design.Internal;
using Microsoft.Extensions.DependencyInjection;
using Migrator.Logic.Models;
using System;
using System.Linq;
using System.Reflection;

namespace Toolbox.Extension.Logic.Migrations
{
    public interface IMigrationService
    {
        int Run(AddMigratorParams addMigratorParams);
    }

    internal class MigrationService : IMigrationService
    {
        public int Run(AddMigratorParams addMigratorParams)
        {
            var processRunner = new ProcessRunner();
            return processRunner.Execute(addMigratorParams);
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
