using Migrator.Logic;
using Migrator.Logic.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

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
                    var types = TypeFinder.GetMigrationByDbContextFromAssebly(commandParams.AssemblyFileName);
                    return writeResult(types);
                }
            }
            catch (InvalidOperationException)
            {
                return ExitCode.CanNotFindDbContext;
            }
            catch (FileNotFoundException e)
            {
                LogException(e);
                return ExitCode.CanNotFindFile;
            }
            catch (ArgumentException e)
            {
                LogException(e);
                return ExitCode.InvalidArguments;
            }
            catch (Exception e)
            {
                LogException(e);
                return ExitCode.Exception;
            }

            return ExitCode.InvalidCommand;
        }

        private static int writeResult(Dictionary<string, List<string>> types)
        {
            var buffer = new StringBuilder();
            foreach(var item in types)
            {
                buffer.Append($"{item.Key};{string.Join(";", item.Value)}|");
            }
            Console.WriteLine(buffer.ToString());
            return ExitCode.Success;
        }

        private static int writeResult(List<string> types)
        {
            Console.WriteLine(string.Join(";", types));
            return ExitCode.Success;
        }

        private static void LogException(Exception exception)
        {
            Console.WriteLine(exception.Message);
        }
    }
}
