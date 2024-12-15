using Microsoft.Extensions.Logging;

namespace CopyTableData.PostgreSql;

public class PostgreSqlFactory : IDatabaseSpecificFactory
{

    private readonly ILoggerFactory _loggerFactory;
    public PostgreSqlFactory(ILoggerFactory loggerFactory)
    {
        _loggerFactory = loggerFactory;
    }

    public ITableReader CreateTableReader(string connectionString)
    {
        return new PostgreSqlTableReader(connectionString);
    }

    public ITableWriter CreateTableWriter(string connectionString)
    {
        return new PostgreSqlTableWriter(connectionString, _loggerFactory.CreateLogger<PostgreSqlTableWriter>());
    }

    public IColumnReader CreateColumnReader(string connectionString)
    {
        return new PostgreSqlColumnReader(connectionString);
    }

    public IScriptExecutor CreateScriptExecutor(string connectionString)
    {
        return new PostgreSqlScriptExecutor(connectionString);
    }
}