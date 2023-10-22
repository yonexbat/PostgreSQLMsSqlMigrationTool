using System.Data.SqlClient;

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
        var sql = $"SELECT column_name FROM INFORMATION_SCHEMA.Columns where TABLE_NAME = '{tableName}'";
        var command = new SqlCommand(sql, connection);
        var reader = command.ExecuteReader();

        var result = new List<string>();
        while (reader.Read())
        {
            result.Add(reader.GetString(0));
        }

        return result;
    }
}