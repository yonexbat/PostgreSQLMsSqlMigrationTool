using Microsoft.Data.SqlClient;

namespace CopyTableData.MsSql;

public class MsSqlScriptExecutor : IScriptExecutor
{
    private readonly string _connectionString;

    public MsSqlScriptExecutor(string connectionString)
    {
        _connectionString = connectionString;
    }
    public void ExecuteScript(string script)
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();

        using var sqlCommand = new SqlCommand(script, connection);
        var res = sqlCommand.ExecuteNonQuery();
    }
}