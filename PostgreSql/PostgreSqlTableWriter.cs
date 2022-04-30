using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostreSQLMsSqlMigrationTool.PostgreSql
{
    internal class PostgreSqlTableWriter : ITableWriter
    {


        private string _connectionString;

        private NpgsqlConnection? _connection;
        private NpgsqlBinaryImporter? _binaryImporter;


        public PostgreSqlTableWriter(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DestinationDatabase");
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

        public void Write(object?[] values)
        {
            if (_binaryImporter == null)
            {
                throw new Exception("call open first");
            }
            _binaryImporter.StartRow();
            foreach (object? value in values)
            {
                if (value == null)
                {
                    _binaryImporter.WriteNull();
                }
                else if (value is DateTimeOffset)
                {
                    DateTimeOffset dateTimeOffset = (DateTimeOffset) value;
                    _binaryImporter.Write(dateTimeOffset.ToUniversalTime());
                }
                else
                {
                    _binaryImporter.Write(value);
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
                        _binaryImporter.Close();
                    }
                    if (_connection != null)
                    {
                        _connection.Close();
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
    }
}
