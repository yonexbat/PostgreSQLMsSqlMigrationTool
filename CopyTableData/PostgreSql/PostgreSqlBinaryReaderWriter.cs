using Microsoft.Extensions.Logging;
using Npgsql;

namespace CopyTableData.PostgreSql;

public class PostgreSqlBinaryReaderWriter(string connectionString, ILoggerFactory loggerFactory) : IBinaryReaderWriter
{
    
    private readonly ILogger _logger = loggerFactory.CreateLogger<PostgreSqlBinaryReaderWriter>();
    
    public IEnumerable<BinaryReadItem> GetBinaries(string tableName, string? idColumn, string binaryColumn)
    {
        using var connection = new NpgsqlConnection(connectionString);
        connection.Open();
        var sql = $"SELECT \"{idColumn}\", \"{binaryColumn}\" FROM \"{tableName}\"";
        using var sqlCommand = new NpgsqlCommand(sql, connection);
        
        var reader = sqlCommand.ExecuteReader();
        if (reader.Read())
        {
            yield return new BinaryReadItem()
            {
                Id   = reader.GetValue(0),
                Stream = reader.GetStream(1),
            };
        }
    }

    public void WriteBinary(string tableName, string? idColumn, string binaryColumn, BinaryReadItem item)
    {
        // Stream data in chunks
        uint chunkSize = 1024 * 1024; // 1 MB
        var buffer = new byte[chunkSize];
        long totalBytesRead = 0;
        int bytesRead;
        
        using var connection = new NpgsqlConnection(connectionString);
        connection.Open();

        {
            using var resetCommand = new NpgsqlCommand($"UPDATE \"{tableName}\" SET \"{binaryColumn}\" = '' WHERE \"{idColumn}\" = @id", connection);
            resetCommand.Parameters.AddWithValue("id", item.Id!);
            resetCommand.ExecuteNonQuery();
        }

        while ((bytesRead =  item.Stream.Read(buffer)) > 0)
        {
            var chunk = buffer[..bytesRead];

            string sql = $"UPDATE \"{tableName}\" SET \"{binaryColumn}\" = \"{binaryColumn}\" || @chunk  WHERE \"{idColumn}\" = @id";

            using var updateCommand = new NpgsqlCommand(sql, connection);
            updateCommand.Parameters.AddWithValue("id", item.Id!);
            updateCommand.Parameters.AddWithValue("chunk", chunk);
        
            updateCommand.ExecuteNonQuery();
        
            totalBytesRead += bytesRead;
            Log.BytesWritten(_logger, totalBytesRead, null);
        }
    }
    
    private class Log
    {
        internal static readonly Action<ILogger, long, Exception?> BytesWritten = LoggerMessage.Define<long>(
            LogLevel.Information,
            new EventId(1002, nameof(BytesWritten)),
            "bytes read/written: {bytesWritten}");
    }
}