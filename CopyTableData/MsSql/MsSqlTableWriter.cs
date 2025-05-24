using Microsoft.Data.SqlClient;

namespace CopyTableData.MsSql;

public class MsSqlTableWriter(string connectionString) : ITableWriter
{
    private IList<string> _colNames;
    private bool _disposedValue;
    private SqlBulkCopy? _sqlBulkCopy;

    private SqlBulkCopy SqlBulkCopy
    {
        get
        {
            if (_sqlBulkCopy == null) throw new InvalidOperationException("Open() not called yet");
            return _sqlBulkCopy;
        }
    }

    public void Open(string tableName, IList<DataBaseColMapping> columns)
    {
        var colNames = columns.Select(x => x.DestinationColName).ToList();
        Open(tableName, colNames);
    }


    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public void WriteAll(ITableReader reader)
    {
        var readerAdapter = new MsSqlReaderAdapter(reader, _colNames);
        SqlBulkCopy.WriteToServer(readerAdapter);
    }

    private void Open(string tableName, IList<string> destinationColNames)
    {
        _sqlBulkCopy = new SqlBulkCopy(connectionString, SqlBulkCopyOptions.KeepIdentity);
        _sqlBulkCopy.DestinationTableName = tableName;
        foreach (var destColName in destinationColNames)
        {
            // MsSqlReaderAdapter maps the colnames of the destination to the index in the array of values of the record.
            // See MsSqlReaderAdapter; public int GetOrdinal(string name)
            _sqlBulkCopy.ColumnMappings.Add(new SqlBulkCopyColumnMapping(destColName, destColName));
        }

        _colNames = destinationColNames;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposedValue) return;
        if (disposing)
            if (_sqlBulkCopy != null)
            {
                IDisposable disposable = _sqlBulkCopy;
                disposable.Dispose();
            }

        _disposedValue = true;
    }
}