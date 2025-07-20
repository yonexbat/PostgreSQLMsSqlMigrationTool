using Microsoft.Extensions.Logging;

namespace CopyTableData.PostgreSql;

public class PostgreSqlFactory(ILoggerFactory loggerFactory) : IDatabaseSpecificFactory
{

    public ITableReader CreateTableReader(string connectionString)
    {
        return new PostgreSqlTableReader(connectionString);
    }

    public ITableWriter CreateTableWriter(string connectionString)
    {
        return new PostgreSqlTableWriter(connectionString, loggerFactory.CreateLogger<PostgreSqlTableWriter>());
    }

    public IColumnReader CreateColumnReader(string connectionString)
    {
        return new PostgreSqlColumnReader(connectionString);
    }

    public IScriptExecutor CreateScriptExecutor(string connectionString)
    {
        return new PostgreSqlScriptExecutor(connectionString);
    }

    public IBinaryReaderWriter CreateBinaryReaderWriter(string connectionString)
    {
        return new PostgreSqlBinaryReaderWriter(connectionString, loggerFactory);
    }
}