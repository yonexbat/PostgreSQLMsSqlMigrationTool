using CopyTableData.MsSql;
using CopyTableData.PostgreSql;
using Microsoft.Extensions.Logging;

namespace CopyTableData;

public class DatabaseReaderWriterFactory
{
    private readonly ConnectionStrings _connectionStrings;
    private readonly IDatabaseSpecificFactory _msSqlFactory;
    private readonly IDatabaseSpecificFactory _postgreSqlFactory;


    public DatabaseReaderWriterFactory(ConnectionStrings connectionStrings, ILoggerFactory loggerFactory)
    {
        _connectionStrings = connectionStrings;
        _msSqlFactory = new MsSqlFactory();
        _postgreSqlFactory = new PostgreSqlFactory(loggerFactory);
    }


    public ITableReader CreateTableReader(string tech)
    {
        return GetFactory(tech).CreateTableReader(_connectionStrings.SourceDatabase);
    }
    
    public IScriptExecutor CreateScriptExecutor(string tech)
    {
        return GetFactory(tech).CreateScriptExecutor(_connectionStrings.DestinationDatabase);
    }

    public ITableWriter CreateTableWriter(string tech)
    {
        return GetFactory(tech).CreateTableWriter(_connectionStrings.DestinationDatabase);
    }

    public IColumnReader CreateColumnReader(string tech, bool isSource)
    {
        var connectionString = isSource ? _connectionStrings.SourceDatabase : _connectionStrings.DestinationDatabase;
        return GetFactory(tech).CreateColumnReader(connectionString);
    }

    private IDatabaseSpecificFactory GetFactory(string tech)
    {
        switch (tech)
        {
            case "mssql":
                return _msSqlFactory;
            case "pgsql":
                return _postgreSqlFactory;
            default:
                throw new NotSupportedException($"DB-technology {tech} not supported.");
        }
    }
}