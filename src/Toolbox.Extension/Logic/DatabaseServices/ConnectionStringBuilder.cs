using System.Data.SqlClient;

namespace Toolbox.Extension.Logic.DatabaseServices
{
    internal class ConnectionStringBuilder
    {
        public static string CreateMsSqlConnectionString(
            string server, string database, bool isSqlAuthentication, string username, string password)
        {
            var builder = new SqlConnectionStringBuilder()
            {
                DataSource = server,
                InitialCatalog = database,
                IntegratedSecurity = !isSqlAuthentication
            };

            if (isSqlAuthentication)
            {
                builder.UserID = username;
                builder.Password = password;
            }
            
            return builder.ConnectionString;
        }
    }
}
