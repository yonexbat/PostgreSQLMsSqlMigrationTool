using Npgsql;

namespace CopyTableData.PostgreSql;

public class PostgreSqlTableReader : ITableReader
{

    private readonly string _connectionString;
    private NpgsqlConnection? _connection;

    private NpgsqlDataReader? _reader;

    private NpgsqlCommand? _sqlCommand;

    private object?[]? _values;
    private bool disposedValue;

    public PostgreSqlTableReader(string connectionString)
    {
        _connectionString = connectionString;
    }

    private int ColCount { get; set; }

    private NpgsqlDataReader Reader
    {
        get
        {
            if (_reader == null) throw new InvalidOperationException("Open() not called yet");
            return _reader;
        }
    }

    public void Open(string tableName, IList<string> colNames)
    {
        ColCount = colNames.Count;
        _connection = new NpgsqlConnection(_connectionString);
        _connection.Open();
        var sql = GetSql(tableName, colNames);
        _sqlCommand = new NpgsqlCommand(sql, _connection);
        _reader = _sqlCommand.ExecuteReader();
        _values = new object?[ColCount];
    }

    public bool Read()
    {
        return Reader.Read();
    }

    public object?[] GetValues()
    {
        if (_reader == null || _values == null) throw new Exception("CaLL Open() and Read() first");

        for (var i = 0; i < ColCount; i++)
            if (!_reader.IsDBNull(i))
                _values[i] = _reader.GetValue(i);
            else
                _values[i] = null;
        return _values;
    }


    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private string GetSql(string tableName, IList<string> colNames)
    {
        colNames = colNames.Select(colName => $"\"{colName}\"").ToList();
        var cols = string.Join(", ", colNames);
        return $"SELECT {cols} FROM \"{tableName}\"";
    }


    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                if (_reader != null) _reader.Dispose();
                if (_sqlCommand != null) _sqlCommand.Dispose();
                if (_connection != null) _connection.Dispose();
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            disposedValue = true;
        }
    }
}