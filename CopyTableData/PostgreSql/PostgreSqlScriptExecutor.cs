using Npgsql;

namespace CopyTableData.PostgreSql;

public class PostgreSqlScriptExecutor(string connectionString) : IScriptExecutor
{
    private readonly string _connectionString = connectionString;

    public void ExecuteScript(string script)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        connection.Open();

        using var sqlCommand = new NpgsqlCommand(script, connection);
        var res = sqlCommand.ExecuteNonQuery();
    }
}