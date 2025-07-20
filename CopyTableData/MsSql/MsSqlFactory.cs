using Microsoft.Extensions.Logging;

namespace CopyTableData.MsSql;

public class MsSqlFactory(ILoggerFactory loggerFactory) : IDatabaseSpecificFactory
{

    public ITableReader CreateTableReader(string connectionString)
    {
        return new MsSqlTableReader(connectionString);
    }

    public ITableWriter CreateTableWriter(string connectionString)
    {
        return new MsSqlTableWriter(connectionString);
    }

    public IColumnReader CreateColumnReader(string connectionString)
    {
        return new MsSqlColumnReader(connectionString);
    }

    public IScriptExecutor CreateScriptExecutor(string connectionString)
    {
        return new MsSqlScriptExecutor(connectionString);
    }

    public IBinaryReaderWriter CreateBinaryReaderWriter(string connectionString)
    {
        return new MsSqlBinaryReaderWrite(connectionString, loggerFactory);
    }
}