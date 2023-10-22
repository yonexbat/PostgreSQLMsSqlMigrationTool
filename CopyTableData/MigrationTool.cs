using Microsoft.Extensions.Logging;

namespace CopyTableData;

public class MigrationTool
{
    private readonly DatabaseReaderWriterFactory _databaseReaderWriterFactory;
    private readonly ILogger _logger;
    private readonly MigrationOptions _migrationOptions;

    public MigrationTool(
        MigrationOptions migrationOptions,
        DatabaseReaderWriterFactory databaseReaderWriterFactory,
        ILoggerFactory loggerFactory)
    {
        _migrationOptions = migrationOptions;
        _databaseReaderWriterFactory = databaseReaderWriterFactory;
        _logger = loggerFactory.CreateLogger<MigrationTool>();
    }

    public void Migrate()
    {
        foreach (var migration in _migrationOptions.MigrationItems)
        {
            MigrateTable(migration);
        }
    }

    private void MigrateTable(MigrationItem migration)
    {
        _logger.StartMigrationItem(migration.SourceTableName, migration.DestinationTableName);

        if (migration.ColMappings == null || !migration.ColMappings.Any())
        {
            CreateColMappingFromDbMetadata(migration);
        }
        
        using var tableReader = OpenNewTableReader(migration);
        using var tableWriter = OpenNewTableWriter(migration);

        tableWriter.WriteAll(tableReader);
    }

    private void CreateColMappingFromDbMetadata(MigrationItem migration)
    {
        _logger.NoColMappingDefined();
        migration.ColMappings ??= new List<ColMapping>();
        
        var colReaderSource = _databaseReaderWriterFactory.CreateColumnReader(_migrationOptions.SourceDbTech, true);
        var sourceCols = colReaderSource.GetColumnNames(migration.SourceTableName);

        var colReaderDestination =
            _databaseReaderWriterFactory.CreateColumnReader(_migrationOptions.DestinationDbTech, false);
        var destinationCols = colReaderDestination.GetColumnNames(migration.DestinationTableName);

        foreach (var sourceCol in sourceCols)
        {
            var destinationCol = destinationCols
                .FirstOrDefault(dc => sourceCol.Equals(dc, StringComparison.OrdinalIgnoreCase));
            if (destinationCol != null)
            {
                ColMapping colMapping = new ColMapping()
                {
                    SourceColName = sourceCol,
                    DestinationColName = destinationCol,
                };
                migration.ColMappings.Add(colMapping);
            }
        }
    }

    private ITableReader OpenNewTableReader(MigrationItem migration)
    {
        if (migration.ColMappings == null || !migration.ColMappings.Any())
        {
            throw new ArgumentException("ColMappings must not be null or empty here");
        }
        
        List<string> colNames = migration.ColMappings.Select(x => x.SourceColName).ToList()!;
        var reader = _databaseReaderWriterFactory.CreateTableReader(_migrationOptions.SourceDbTech);
        reader.Open(migration.SourceTableName, colNames);
        return reader;
    }

    private ITableWriter OpenNewTableWriter(MigrationItem migration)
    {
        if (migration.ColMappings == null || !migration.ColMappings.Any())
        {
            throw new ArgumentException("ColMappings must not be null or empty here");
        }
        List<string> colNamesDest = migration.ColMappings.Select(x => x.DestinationColName).ToList()!;
        var writer = _databaseReaderWriterFactory.CreateTableWriter(_migrationOptions.DestinationDbTech);
        writer.Open(migration.DestinationTableName, colNamesDest);
        return writer;
    }

  
}

internal static class Log
{
    public static void StartMigrationItem(this ILogger logger, string from , string to)
    {
        _startMigrationItem(logger, from, to, default!);
    }

    public static void NoColMappingDefined(this ILogger logger)
    {
        _noColMappingDefined(logger, default!);
    }
        
    internal static readonly Action<ILogger, string, string, Exception> _startMigrationItem =
        LoggerMessage.Define<string, string>(
            LogLevel.Information,
            new EventId(1, nameof(StartMigrationItem)),
            "Starting migration from {from} to {to}");
        
    internal static readonly Action<ILogger, Exception> _noColMappingDefined =
        LoggerMessage.Define(
            LogLevel.Information,
            new EventId(2, nameof(NoColMappingDefined)),
            "No col mapping defined for migration. Will lookup metadata in db.");        
}