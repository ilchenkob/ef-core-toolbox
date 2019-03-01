using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Reflection;

namespace Migrator.Logic
{
    internal class DbContextFactory
    {
        private const string ConnectionStringStub = "connection_string";

        public static DbContext CreateContextInstance(string assemblyFileName, string contextFullName)
        {
            var assembly = Assembly.LoadFile(assemblyFileName);
            var contextType = assembly.GetExportedTypes().FirstOrDefault(t => t.FullName == contextFullName);
            try
            {
                return (DbContext)Activator.CreateInstance(contextType);
            }
            catch (MissingMethodException)
            {
                try
                {
                    return (DbContext)Activator.CreateInstance(contextType, ConnectionStringStub);
                }
                catch (MissingMethodException)
                {
                    return (DbContext)Activator.CreateInstance(contextType, ConnectionStringStub, ConnectionStringStub);
                }
            }
        }
    }
}
