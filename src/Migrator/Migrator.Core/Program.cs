using Migrator.Logic;
using Migrator.Logic.Models;

namespace Migrator.Core
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args == null || args.Length < 1)
                return ExitCode.InvalidArguments;

            if (args[0] == CommandType.AddMigration)
            {
                var p = new AddMigratorParams
                {
                    AssemblyFileName = @"C:\Users\Vitalii_Ilchenko\source\repos\ConsoleApp9\ConsoleAppcore\bin\Debug\netcoreapp2.1\ConsoleAppcore.dll",
                    ContextNamespace = "ConsoleAppcore.Test1",
                    DbContextTypeFullName = "ConsoleAppcore.Test1.DbContextTest1",
                    MigrationName = "Test_2341",
                    OutputDir = @"C:\Users\Vitalii_Ilchenko\source\repos\ConsoleApp9\ConsoleAppcore\Migrations",
                    ProjectDir = @"C:\Users\Vitalii_Ilchenko\source\repos\ConsoleApp9\ConsoleAppcore",
                    SubNamespace = "MigrationsSpace"
                };

                var executor = new AddMigrationExecutor();
                return executor.Run(p); //AddMigratorParams.FromArgumentsArray(args)
            }
            else if (args[0] == CommandType.ScriptMigration)
            {
                // TODO: make script
            }

            return ExitCode.InvalidCommand;
        }
    }
}
