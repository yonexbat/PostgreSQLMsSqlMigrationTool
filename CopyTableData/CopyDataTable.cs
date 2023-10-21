using Microsoft.Extensions.Logging;

namespace CopyTableData;

public static class CopyDataTable
{
    public static void CopyTables(ConnectionStrings connectionStrings, MigrationOptions migrationOptions)
    {
        var loggerFactory = LoggerFactory.Create(loggerBuilder => loggerBuilder.AddConsole());
        var readerWriterFactory = new DatabaseReaderWriterFactory(connectionStrings, loggerFactory);
        MigrationTool migrationTool = new MigrationTool(migrationOptions, readerWriterFactory, loggerFactory);
        migrationTool.Migrate();
    }
}