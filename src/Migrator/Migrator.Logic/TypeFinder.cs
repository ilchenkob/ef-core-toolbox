using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
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
            return getPublicTypesFromAssebly(assembly, typeof(DbContext))
                .Select(t => t.FullName)
                .ToList();
        }

        public static Dictionary<string, List<string>> GetMigrationByDbContextFromAssebly(string assembly)
        {
            return getPublicTypesFromAssebly(assembly, typeof(Migration))
                .Select(m =>
                {
                    var contextAttribute = m.GetCustomAttribute<DbContextAttribute>();
                    var migrationAttribute = m.GetCustomAttribute<MigrationAttribute>();
                    return
                    (
                        migrationName: migrationAttribute == null ? m.Name : migrationAttribute.Id,
                        contextFullName: contextAttribute == null ? string.Empty : contextAttribute.ContextType.FullName
                    );
                })
                .Where(i => !string.IsNullOrEmpty(i.contextFullName))
                .GroupBy(i => i.contextFullName)
                .ToDictionary(g => g.Key, g => g.Select(m => m.migrationName).ToList());
        }

        private static IEnumerable<Type> getPublicTypesFromAssebly(string assembly, Type inheritedFromType)
        {
            if (!File.Exists(assembly))
                throw new FileNotFoundException($"File is not found: {assembly}");

            var assemblyInstance = Assembly.LoadFile(assembly);
            if (assemblyInstance == null)
                throw new FileLoadException($"Can not load the assembly: {assembly}");

            var exportedTypes = assemblyInstance.GetExportedTypes();
            if (exportedTypes == null || exportedTypes.Length == 0)
                throw new InvalidOperationException($"Assembly does not have any public types: {assembly}");

            return exportedTypes.Where(t => t?.BaseType != null && t.BaseType == inheritedFromType);
        }
    }
}
