using Migrator.Logic;
using Migrator.Logic.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace Migrator.Core
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args == null || args.Length == 0)
                return ExitCode.InvalidArguments;

            try
            {
                if (args[0] == CommandType.AddMigration)
                {
                    var commandParams = AddMigrationParams.FromArgumentsArray(args);
                    return new AddMigrationExecutor().Run(commandParams);
                }
                else if (args[0] == CommandType.ScriptMigration)
                {
                    var commandParams = ScriptMigrationParams.FromArgumentsArray(args);
                    return new ScriptMigrationExecutor().Run(commandParams);
                }
                else if (args[0] == CommandType.FindDbContextSubtypes)
                {
                    var commandParams = FindDbContextSubtypeParams.FromArgumentsArray(args);
                    var types = TypeFinder.GetDbContextTypeFullNamesFromAssebly(commandParams.AssemblyFileName);
                    return writeResult(types);
                }
                else if (args[0] == CommandType.FindMigrationSubtypes)
                {
                    var commandParams = FindMigrationSubtypeParams.FromArgumentsArray(args);
                    var types = TypeFinder.GetMigrationTypeFullNamesFromAssebly(commandParams.AssemblyFileName);
                    return writeResult(types);
                }
            }
            catch (InvalidOperationException)
            {
                return ExitCode.CanNotFindDbContext;
            }
            catch (FileNotFoundException exception)
            {
                Console.WriteLine(exception.Message);
                return ExitCode.CanNotFindFile;
            }
            catch (ArgumentException exception)
            {
                Console.WriteLine(exception.Message);
                return ExitCode.InvalidArguments;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                return ExitCode.Exception;
            }

            return ExitCode.InvalidCommand;
        }

        private static int writeResult(List<string> types)
        {
            Console.WriteLine(string.Join(";", types));
            return ExitCode.Success;
        }
    }
}
