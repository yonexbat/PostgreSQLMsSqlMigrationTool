﻿using Microsoft.Data.SqlClient;

namespace CopyTableData.MsSql;

public class MsSqlTableWriter : ITableWriter
{
    private readonly string _connectionString;
    private bool _disposedValue;
    private int _fieldCount;
    private SqlBulkCopy? _sqlBulkCopy;

    public MsSqlTableWriter(string connectionString)
    {
        _connectionString = connectionString;
    }

    public SqlBulkCopy SqlBulkCopy
    {
        get
        {
            if (_sqlBulkCopy == null) throw new InvalidOperationException("Open() not called yet");
            return _sqlBulkCopy;
        }
    }

    public void Open(string tableName, IList<string> colNames)
    {
        _sqlBulkCopy = new SqlBulkCopy(_connectionString);
        _sqlBulkCopy.DestinationTableName = tableName;
        _fieldCount = colNames.Count;
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
        var readerAdapter = new MsSqlReaderAdapter(reader, _fieldCount);
        SqlBulkCopy.WriteToServer(readerAdapter);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
                if (_sqlBulkCopy != null)
                {
                    IDisposable disposable = _sqlBulkCopy;
                    disposable.Dispose();
                }

            _disposedValue = true;
        }
    }
}