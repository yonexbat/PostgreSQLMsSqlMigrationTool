namespace CopyTableData.MsSql;

public class MsSqlFactory : IDatabaseSpecificFactory
{

    public ITableReader CreateTableReader(string connectionString)
    {
        return new  MsSqlTableReader(connectionString);
    }

    public ITableWriter CreateTableWriter(string connectionString)
    {
        return new MsSqlTableWriter(connectionString);
    }

    public IColumnReader CreateColumnReader(string connectionString)
    {
        return new MsSqlColumnReader(connectionString);
    }
}