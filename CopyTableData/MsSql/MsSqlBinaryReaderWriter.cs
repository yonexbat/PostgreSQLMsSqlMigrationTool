using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace CopyTableData.MsSql;

public class MsSqlBinaryReaderWrite(string connectionString, ILoggerFactory loggerFactory) : IBinaryReaderWriter
{
    private readonly ILogger _logger = loggerFactory.CreateLogger<MsSqlBinaryReaderWrite>();
    public IEnumerable<BinaryReadItem> GetBinaries(string tableName, string? idColumn, string binaryColumn)
    {
        using SqlConnection connection = new SqlConnection(connectionString);
        connection.Open();
        var sql = $"SELECT [{idColumn}], [{binaryColumn}] FROM [{tableName}]";
        using var command = new SqlCommand(sql, connection);
        using var reader = command.ExecuteReader();

        if (reader.Read())
        {
            yield return new BinaryReadItem()
            {
                Id = reader.GetValue(0),
                Stream = reader.GetStream(1),
            };
        }
    }

    public void WriteBinary(string tableName, string? idColumn, string binaryColumn, BinaryReadItem item)
    {
        using SqlConnection connection = new SqlConnection(connectionString);
        connection.Open();

        uint chunkSize = 1024 * 1024; // 1 MB
        var buffer = new byte[chunkSize];
        long totalBytesRead = 0;
        int bytesRead;
        
        {
            using var resetCommand = new SqlCommand($"UPDATE [{tableName}] SET [{binaryColumn}] = NULL WHERE [{idColumn}] = @id", connection);
            resetCommand.Parameters.AddWithValue("id", item.Id!);
            resetCommand.ExecuteNonQuery();
        }
        
        while ((bytesRead =  item.Stream.Read(buffer)) > 0)
        {
            var chunk = buffer[..bytesRead];

            string sql = $"UPDATE [{tableName}] SET [{binaryColumn}] = [{binaryColumn}] + @chunk  WHERE [{idColumn}] = @id";

            using var updateCommand = new SqlCommand(sql, connection);
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
            new EventId(1001, nameof(BytesWritten)),
            "bytes read/written: {bytesWritten}");
    }
}