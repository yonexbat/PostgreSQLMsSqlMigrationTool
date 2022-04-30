using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostreSQLMsSqlMigrationTool.PostgreSql
{
    internal class PostgreSqlTableWriter : IDisposable
    {


        private string _connectionString;

        private string _tableName;
        IList<string> _colNames;
        private NpgsqlConnection? _connection;
        private NpgsqlBinaryImporter? _binaryImporter;


        public PostgreSqlTableWriter(string connectionString, string tableName, IList<string> colNames)
        {
            _connectionString = connectionString;
            _tableName = tableName;
            _colNames = colNames;
        }


        private bool disposedValue;

        public void Open()
        {
            _connection = new NpgsqlConnection(_connectionString);
            _connection.Open();

            string colNames = string.Join(", ", _colNames);
            string bulkInsertSql = $"COPY {_tableName}({colNames}) from STDIN (format binary)";
            _binaryImporter = _connection.BeginBinaryImport(bulkInsertSql);

        }

        public void Write(IList<object?> values)
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
