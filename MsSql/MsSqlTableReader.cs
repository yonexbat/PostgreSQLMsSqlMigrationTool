using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostreSQLMsSqlMigrationTool.MsSql
{
    internal class MsSqlTableReader : ITableReader
    {
        private bool disposedValue;

        private SqlConnection? _connection;

        private SqlCommand? _sqlCommand;

        private SqlDataReader? _reader;

        private string _connectionString;

        private int ColCount { get; set; }

        private object?[]? _values;

        private SqlDataReader Reader
        {
            get 
            { 
                if(_reader == null)
                {
                    throw new InvalidOperationException("Open() not called yet");
                }
                return _reader; 
            }
        }

        public MsSqlTableReader(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("SourceDatabase");
        }

        public void Open(string tableName, IList<string> colNames)
        {
            ColCount = colNames.Count;
            _connection = new SqlConnection(_connectionString);
            _connection.Open();
            string sql = GetSql(tableName, colNames);
            _sqlCommand = new SqlCommand(sql, _connection);
            _reader = _sqlCommand.ExecuteReader();
            _values = new object?[ColCount];
        }

        private string GetSql(string tableName, IList<string> colNames)
        {
            string cols = string.Join(", ", colNames);
            return $"SELECT {cols} FROM {tableName}";
        }

        public bool Read()
        {
            return Reader.Read();
        }

        public object?[] GetValues()
        {
            if (_reader == null || _values == null)
            {
                throw new Exception("CaLL Open() and Read() first");
            }

            for (int i = 0; i < ColCount; i++)
            {
                if (!_reader.IsDBNull(i))
                {
                    _values[i] = _reader.GetValue(i);
                }
                else
                {
                    _values[i] = null;
                }
            }
            return _values;
        }



        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (_reader != null)
                    {
                        _reader.Dispose();
                    }
                    if (_sqlCommand != null)
                    {
                        _sqlCommand.Dispose();
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
    }
}
