namespace CopyTableData;

public interface ITableWriter : IDisposable
{
    void Open(string tableName, IList<string> colNames);

    public void WriteAll(ITableReader reader);
}