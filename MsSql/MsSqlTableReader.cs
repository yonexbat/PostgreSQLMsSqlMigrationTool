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
        }

        private string GetSql(string tableName, IList<string> colNames)
        {
            string cols = string.Join(", ", colNames);
            return $"SELECT {cols} FROM {tableName}";
        }

        public bool Read()
        {
            if (_reader != null)
            {
                return _reader.Read();
            }
            throw new Exception("CaLL Open() first");
        }

        public IList<object?> GetValues()
        {
            if (_reader == null)
            {
                throw new Exception("CaLL Open()  and Read() first");
            }

            IList<object?> values = new List<object?>();
            for (int i = 0; i < ColCount; i++)
            {
                if (!_reader.IsDBNull(i))
                {
                    values.Add(_reader.GetValue(i));
                }
                else
                {
                    values.Add(null);
                }
            }
            return values;
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
