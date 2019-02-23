﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Design.Internal;
using Microsoft.EntityFrameworkCore.Scaffolding;
using Microsoft.EntityFrameworkCore.SqlServer.Design.Internal;
using Microsoft.Extensions.DependencyInjection;
using Toolbox.Extension.Logic.Scaffolding.Models;

namespace Toolbox.Extension.Logic.Scaffolding
{
    internal class ScaffoldingService : IScaffoldingService
    {
        private const string ProgrammingLanguage = "C#";

        public bool ScaffoldDatabase(ScaffoldingExecutorParams scaffoldingParams)
        {
            try
            {
                var serviceCollection = new ServiceCollection();

                serviceCollection
                    .AddEntityFrameworkDesignTimeServices()
                    .AddSingleton<IOperationReporter, OperationReporter>()
                    .AddSingleton<IOperationReportHandler, OperationReportHandler>();

                var designTimeServices = new SqlServerDesignTimeServices();
                designTimeServices.ConfigureDesignTimeServices(serviceCollection);

                var serviceProvider = serviceCollection.BuildServiceProvider();
                var scaffolder = serviceProvider.GetService<IReverseEngineerScaffolder>();

                var scaffoldingModel = scaffolder.ScaffoldModel(
                        language: ProgrammingLanguage,
                        connectionString: scaffoldingParams.ConnectionString,
                        tables: scaffoldingParams.Tables,
                        schemas: new List<string>(),
                        @namespace: scaffoldingParams.DbContextNamespace,
                        contextName: scaffoldingParams.DbContextClassName,
                        codeOptions: new ModelCodeGenerationOptions
                        {
                            SuppressConnectionStringWarning = false,
                            UseDataAnnotations = scaffoldingParams.UseDataAnnotations
                        },
                        modelOptions: new ModelReverseEngineerOptions
                        {
                            UseDatabaseNames = scaffoldingParams.UseDatabaseNames
                        },
                        contextDir: scaffoldingParams.OutputFolder);

                var saveResult = scaffolder.Save(
                        scaffoldingModel,
                        outputDir: scaffoldingParams.OutputFolder,
                        overwriteFiles: true);
              
                return true;
            }
            catch (Exception ex)
            {
                var a = ex.Message;
                return false;
            }
        }
    }
}
