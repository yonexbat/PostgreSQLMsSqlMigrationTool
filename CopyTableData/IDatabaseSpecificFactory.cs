namespace CopyTableData;

public interface IDatabaseSpecificFactory
{
    public ITableReader CreateTableReader(string connectionString);

    public ITableWriter CreateTableWriter(string connectionString);

    public IColumnReader CreateColumnReader(string connectionString);
}