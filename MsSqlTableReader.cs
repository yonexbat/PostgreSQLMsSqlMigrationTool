using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostreSQLMsSqlMigrationTool
{
    internal class MsSqlTableReader : IDisposable
    {
        private bool disposedValue;

        private SqlConnection? _connection;

        private SqlCommand? _sqlCommand;

        private SqlDataReader? _reader;

        private string _connectionString;

        private string _tableName;
        IList<string> _colNames;

        public MsSqlTableReader(string connectionString, string tableName, IList<string> colNames)
        {
            _connectionString = connectionString;
            _tableName = tableName;
            _colNames = colNames;
        }

        public void Open()
        {
            _connection = new SqlConnection(_connectionString);
            _connection.Open();
            string sql = GetSql();
            _sqlCommand = new SqlCommand(sql, _connection);
            _reader = _sqlCommand.ExecuteReader();
            
        }

        private string GetSql()
        {
            string cols = string.Join(", ", _colNames);
            return $"SELECT {cols} FROM {_tableName}";
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
            if(_reader == null)
            {
                throw new Exception("CaLL Open()  and Read() first");
            }            

            IList<object?> values = new List<object?>();
            for(int i = 0; i < _colNames.Count; i++)
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
                    if(_reader != null)
                    {
                        _reader.Close();
                    }
                    if(_sqlCommand != null)
                    {
                        _sqlCommand.Dispose();
                    }
                    if(_connection != null)
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
