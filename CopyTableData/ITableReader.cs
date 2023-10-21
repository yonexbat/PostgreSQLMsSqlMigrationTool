namespace CopyTableData;

public interface ITableReader : IDisposable
{
    void Open(string tableName, IList<string> colNames);

    bool Read();

    object?[] GetValues();
}