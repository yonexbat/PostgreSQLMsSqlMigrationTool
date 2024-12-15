namespace CopyTableData;

public interface ITableWriter : IDisposable
{
    void Open(string tableName, IList<DataBaseColMapping> columns);

    public void WriteAll(ITableReader reader);
}