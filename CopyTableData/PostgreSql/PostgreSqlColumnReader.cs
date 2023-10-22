using Npgsql;

namespace CopyTableData.PostgreSql;

public class PostgreSqlColumnReader : IColumnReader
{
    private readonly string _connectionString;
    public PostgreSqlColumnReader(string connectionString)
    {
        _connectionString = connectionString;
    }
    public IList<string> GetColumnNames(string tableName)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        connection.Open();
        var sql = $"SELECT \"column_name\" FROM information_schema.columns WHERE table_name = '{tableName}';";
        var command = new NpgsqlCommand(sql, connection);
        var reader = command.ExecuteReader();
        
        var result = new List<string>();
        while (reader.Read())
        {
            result.Add(reader.GetString(0));
        }

        return result;
    }
}