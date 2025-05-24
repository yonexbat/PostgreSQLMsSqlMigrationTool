using Npgsql;

namespace CopyTableData.PostgreSql;

public class PostgreSqlColumnReader : IColumnReader
{
    private readonly string _connectionString;
    public PostgreSqlColumnReader(string connectionString)
    {
        _connectionString = connectionString;
    }

    public IList<DataBaseCol> GetDataBaseCols(string tableName)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        connection.Open();
        var sql = $"SELECT \"column_name\", \"data_type\" FROM information_schema.columns WHERE table_name = '{tableName}';";
        var command = new NpgsqlCommand(sql, connection);
        var reader = command.ExecuteReader();

        var result = new List<DataBaseCol>();
        while (reader.Read())
        {
            var colName = reader.GetString(0);
            var colType = reader.GetString(1);
            result.Add(new DataBaseCol(colName, colType));
        }

        return result;
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