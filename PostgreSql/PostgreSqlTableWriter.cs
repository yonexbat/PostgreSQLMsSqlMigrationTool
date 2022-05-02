using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace PostreSQLMsSqlMigrationTool.PostgreSql
{
    internal class PostgreSqlTableWriter : ITableWriter
    {


        private string _connectionString;

        private NpgsqlConnection? _connection;
        private NpgsqlBinaryImporter? _binaryImporter;
        private ILogger _logger;

        private NpgsqlBinaryImporter BinaryImporter
        {
            get
            {
                if(_binaryImporter == null)
                {
                    throw new InvalidOperationException("Open() not called yet!");
                }
                return _binaryImporter;
            }
        }


        public PostgreSqlTableWriter(IConfiguration configuration, ILogger<PostgreSqlTableWriter> logger)
        {
            _connectionString = configuration.GetConnectionString("DestinationDatabase");
            _logger = logger;
        }


        private bool disposedValue;

        public void Open(string tableName, IList<string> colNames)
        {
            _connection = new NpgsqlConnection(_connectionString);
            _connection.Open();

            string colNamesJoined = string.Join(", ", colNames);
            string bulkInsertSql = $"COPY {tableName}({colNamesJoined}) from STDIN (format binary)";
            _binaryImporter = _connection.BeginBinaryImport(bulkInsertSql);

        }

        private void Write(object?[] values)
        {
            BinaryImporter.StartRow();
            foreach (object? value in values)
            {
                if (value == null)
                {
                    BinaryImporter.WriteNull();
                }
                else if (value is DateTimeOffset)
                {
                    DateTimeOffset dateTimeOffset = (DateTimeOffset) value;
                    BinaryImporter.Write(dateTimeOffset.ToUniversalTime());
                }
                else
                {
                    BinaryImporter.Write(value);
                }
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
                    if (_connection != null)
                    {
                        _connection.Dispose();
                    }
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }


        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public void WriteAll(ITableReader tableReader)
        {
            int counter = 0;

            while (tableReader.Read())
            {
                counter++;
                var values = tableReader.GetValues();
                Write(values);
                if (counter % 100 == 0)
                {
                    Log.CountInfo(_logger, counter, default!);
                }
            }
            Log.CountInfo(_logger, counter, default!);
        }

        internal class Log
        {
            static internal readonly Action<ILogger, int, Exception> CountInfo = LoggerMessage.Define<int>(
              LogLevel.Information,
              new EventId(2, "Count Info"),
              "Migrated {numrows} rows");
        }
    }
}
