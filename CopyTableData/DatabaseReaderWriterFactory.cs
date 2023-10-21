using CopyTableData.MsSql;
using CopyTableData.PostgreSql;
using Microsoft.Extensions.Logging;

namespace CopyTableData;

public class DatabaseReaderWriterFactory
{
    private readonly ConnectionStrings _connectionStrings;
    private readonly ILoggerFactory _loggerFactory;

    public DatabaseReaderWriterFactory(ConnectionStrings connectionStrings, ILoggerFactory loggerFactory)
    {
        _loggerFactory = loggerFactory;
        _connectionStrings = connectionStrings;
    }


    public ITableReader CreateTableReader(string tech)
    {
        ITableReader? reader = null;
        switch (tech)
        {
            case "mssql":
                reader = new MsSqlTableReader(_connectionStrings.SourceDatabase);
                break;
            case "pgsql":
                reader = new PostgreSqlTableReader(_connectionStrings.SourceDatabase);
                break;
        }

        if (reader == null) throw new NotSupportedException($"DB-technology {tech} not supported.");

        return reader;
    }

    public ITableWriter CreateTableWriter(string tech)
    {
        ITableWriter? writer = null;
        switch (tech)
        {
            case "pgsql":
                writer = new PostgreSqlTableWriter(_connectionStrings.DestinationDatabase, _loggerFactory.CreateLogger<PostgreSqlTableWriter>());
                break;
            case "mssql":
                writer = new MsSqlTableWriter(_connectionStrings.DestinationDatabase);
                break;
        }

        if (writer == null) throw new NotSupportedException($"DB-technology {tech} not supported.");

        return writer;
    }
}