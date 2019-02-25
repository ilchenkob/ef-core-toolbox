using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

namespace Toolbox.Extension.Logic.DatabaseServices
{
    internal class MsSqlServerService : IDatabaseService, IDatabaseConnector
    {
        public async Task<Dictionary<string, List<string>>> GetTables(string connectionString, CancellationToken cancellationToken)
        {
            var query = @"SELECT schm.name, obj.name
                        FROM [sys].[objects] as obj
                        JOIN [sys].[schemas] as schm ON obj.schema_id = schm.schema_id
                        WHERE type_desc = 'USER_TABLE' ORDER BY obj.name";

            var result = new Dictionary<string, List<string>>();
            try
            {
                using (var connection = new SqlConnection(connectionString))
                using (var command = new SqlCommand(query, connection))
                {
                    await connection.OpenAsync();
                    var reader = await command.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        var schemaName = reader[0]?.ToString();
                        var tableName = reader[1]?.ToString();
                        if (!string.IsNullOrWhiteSpace(schemaName) && !string.IsNullOrWhiteSpace(tableName))
                        {
                            if (!result.ContainsKey(schemaName))
                                result.Add(schemaName, new List<string>());

                            result[schemaName].Add(tableName);
                        }
                    }
                    connection.Close();
                }
            }
            catch
            {
                // TODO: log error
            }
            return result;
        }

        public async Task<bool> TryConnect(string connectionString, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(connectionString)) return false;

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync(cancellationToken);
                    return connection.State == System.Data.ConnectionState.Open;
                }
            }
            catch (SqlException)
            {
                return false;
            }
        }
    }
}
