using System;

namespace Toolbox.Extension
{
    internal static class Constants
    {
        public const string ProgrammingLanguage = "C#";

        public const string DefaultDbContextClassName = "DbContext";

        public static TimeSpan DefaultTaskTimeout => TimeSpan.FromSeconds(15);

        public const string MigratorAssembly = "Migrator.Core.dll";
    }
}
