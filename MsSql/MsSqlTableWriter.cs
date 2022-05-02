using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostreSQLMsSqlMigrationTool.MsSql
{
    internal class MsSqlTableWriter : ITableWriter
    {
        private string _connectionString;
        private SqlBulkCopy? _sqlBulkCopy;
        private int _fieldCount = 0;
        private bool disposedValue;

        public SqlBulkCopy SqlBulkCopy
        {
            get
            {
                if(_sqlBulkCopy == null)
                {
                    throw new InvalidOperationException("Open() not called yet");
                }
                return _sqlBulkCopy;
            }
        }

        public MsSqlTableWriter(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DestinationDatabase");
        }

        public void Open(string tableName, IList<string> colNames)
        {
            _sqlBulkCopy = new SqlBulkCopy(_connectionString);
            _sqlBulkCopy.DestinationTableName = tableName;
            _fieldCount = colNames.Count;          
        }      

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (_sqlBulkCopy != null)
                    {
                        IDisposable disposable = _sqlBulkCopy;
                        disposable.Dispose();
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

        public void WriteAll(ITableReader reader)
        {
            var readerAdapter = new MsSqlReaderAdapter(reader, _fieldCount);
            SqlBulkCopy.WriteToServer(readerAdapter);
        }
    }
}
