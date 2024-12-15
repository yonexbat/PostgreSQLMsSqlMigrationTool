using Microsoft.Data.SqlClient;

namespace CopyTableData.MsSql;

public class MsSqlColumnReader : IColumnReader
{
    private readonly string _connectionString;
    public MsSqlColumnReader(string connectionString)
    {
        _connectionString = connectionString;
    }
    public IList<string> GetColumnNames(string tableName)
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();
        var sql = "SELECT column_name FROM INFORMATION_SCHEMA.Columns where TABLE_NAME = @tableName";
        var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@tableName", tableName);
        var reader = command.ExecuteReader();

        var result = new List<string>();
        while (reader.Read())
        {
            result.Add(reader.GetString(0));
        }

        return result;
    }

    public IList<DataBaseCol> GetDataBaseCols(string tableName)
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();
        var sql = "SELECT column_name, data_type FROM INFORMATION_SCHEMA.Columns where TABLE_NAME = @tableName";
        var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@tableName", tableName);
        var reader = command.ExecuteReader();

        var result = new List<DataBaseCol>();
        while (reader.Read())
        {
            string colName = reader.GetString(0);
            string colType = reader.GetString(1);
            result.Add(new DataBaseCol(colName, colType));
        }

        return result;
    }
}