using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Migrator.Logic
{
    public class TypeFinder
    {
        public static List<string> GetDbContextTypeFullNamesFromAssebly(string assembly)
        {
            return getTypeFullNamesFromAssebly(assembly, typeof(DbContext));
        }

        public static List<string> GetMigrationTypeFullNamesFromAssebly(string assembly)
        {
            return getTypeFullNamesFromAssebly(assembly, typeof(Migration));
        }

        private static List<string> getTypeFullNamesFromAssebly(string assembly, Type inheritedFromType)
        {
            if (!File.Exists(assembly))
                throw new FileNotFoundException($"File is not found: {assembly}");

            var assemblyInstance = Assembly.LoadFile(assembly);
            if (assemblyInstance == null)
                throw new FileLoadException($"Can not load the assembly: {assembly}");

            var exportedTypes = assemblyInstance.GetExportedTypes();
            if (exportedTypes == null || exportedTypes.Length == 0)
                throw new InvalidOperationException($"Assembly does not have any public types: {assembly}");

            return exportedTypes.Where(t => t?.BaseType != null && t.BaseType == inheritedFromType)
                                .Select(t => t.FullName)
                                .ToList();
        }
    }
}
