using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace CopyTableData.PostgreSql;

public class PostgreSqlTableWriter : ITableWriter
{
    private NpgsqlBinaryImporter? _binaryImporter;

    private NpgsqlConnection? _connection;


    private readonly string _connectionString;
    private readonly ILogger _logger;


    private bool disposedValue;


    public PostgreSqlTableWriter(string connectionString, ILogger<PostgreSqlTableWriter> logger)
    {
        _connectionString = connectionString;
        _logger = logger;
    }

    private NpgsqlBinaryImporter BinaryImporter
    {
        get
        {
            if (_binaryImporter == null) throw new InvalidOperationException("Open() not called yet!");
            return _binaryImporter;
        }
    }

    public void Open(string tableName, IList<string> colNames)
    {
        _connection = new NpgsqlConnection(_connectionString);
        _connection.Open();

        var colNamesJoined = string.Join(", ", colNames);
        var bulkInsertSql = $"COPY {tableName}({colNamesJoined}) from STDIN (format binary)";
        _binaryImporter = _connection.BeginBinaryImport(bulkInsertSql);
    }


    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public void WriteAll(ITableReader tableReader)
    {
        var counter = 0;

        while (tableReader.Read())
        {
            counter++;
            var values = tableReader.GetValues();
            Write(values);
            if (counter % 100 == 0) Log.CountInfo(_logger, counter, default!);
        }

        Log.CountInfo(_logger, counter, default!);
    }

    private void Write(object?[] values)
    {
        BinaryImporter.StartRow();
        foreach (var value in values)
            if (value == null)
            {
                BinaryImporter.WriteNull();
            }
            else if (value is DateTimeOffset)
            {
                var dateTimeOffset = (DateTimeOffset)value;
                BinaryImporter.Write(dateTimeOffset.ToUniversalTime());
            }
            else
            {
                BinaryImporter.Write(value);
            }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                if (_binaryImporter != null)
                {
                    _binaryImporter.Complete();
                    _binaryImporter.Dispose();
                }

                if (_connection != null) _connection.Dispose();
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            disposedValue = true;
        }
    }

    internal class Log
    {
        internal static readonly Action<ILogger, int, Exception> CountInfo = LoggerMessage.Define<int>(
            LogLevel.Information,
            new EventId(2, "Count Info"),
            "Migrated {numrows} rows");
    }
}