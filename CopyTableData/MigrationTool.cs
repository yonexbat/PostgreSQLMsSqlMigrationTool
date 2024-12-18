﻿using Microsoft.Extensions.Logging;

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
        if (!string.IsNullOrWhiteSpace(_migrationOptions.DestinationPreScript))
        {
            _logger.ExecutingDestinationScript(_migrationOptions.DestinationPreScript);
            var reader = _databaseReaderWriterFactory.CreateScriptExecutor(_migrationOptions.DestinationDbTech);
            reader.ExecuteScript(_migrationOptions.DestinationPreScript);
        }
        
        foreach (var migration in _migrationOptions.MigrationItems)
        {
            MigrateTable(migration);
        }
        
        if (!string.IsNullOrWhiteSpace(_migrationOptions.DestinationPostScript))
        {
            _logger.ExecutingDestinationScript(_migrationOptions.DestinationPreScript);
            var reader = _databaseReaderWriterFactory.CreateScriptExecutor(_migrationOptions.DestinationDbTech);
            reader.ExecuteScript(_migrationOptions.DestinationPostScript);
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
        var sourceCols = colReaderSource.GetDataBaseCols(migration.SourceTableName);

        var colReaderDestination =
            _databaseReaderWriterFactory.CreateColumnReader(_migrationOptions.DestinationDbTech, false);
        var destinationCols = colReaderDestination.GetDataBaseCols(migration.DestinationTableName);

        foreach (var sourceCol in sourceCols)
        {
            var destinationCol = destinationCols
                .FirstOrDefault(dc => sourceCol.ColumnName.Equals(dc.ColumnName, StringComparison.OrdinalIgnoreCase));
            if (destinationCol != null)
            {
                var colMapping = new ColMapping
                {
                    SourceColName = sourceCol.ColumnName,
                    DestinationColName = destinationCol.ColumnName,
                    SourceColType = sourceCol.DataType,
                    DestinationColType = destinationCol.DataType
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

        List<DataBaseColMapping> colNamesDest = migration.ColMappings.Select(x => new DataBaseColMapping(x.SourceColName, x.SourceColType, x.DestinationColName, x.DestinationColType)).ToList()!;
        var writer = _databaseReaderWriterFactory.CreateTableWriter(_migrationOptions.DestinationDbTech);
        writer.Open(migration.DestinationTableName, colNamesDest);
        return writer;
    }
}

public static partial class Log
{
    [LoggerMessage(
        EventId = 1,
        Level = LogLevel.Information,
        Message = "Starting migration from table {from} to table {to}")]
    public static partial void StartMigrationItem(this ILogger logger, string from, string to);

    [LoggerMessage(
        EventId = 2,
        EventName = nameof(NoColMappingDefined),
        Level = LogLevel.Information,
        Message = "No col mapping defined for migration. Will lookup metadata in db and create col mapping.")]
    public static partial void NoColMappingDefined(this ILogger logger);
    
    [LoggerMessage(
        EventId = 3,
        EventName = nameof(ExecutingDestinationScript),
        Level = LogLevel.Information,
        Message = "Executing SQL on destination database: {script}")]
    public static partial void ExecutingDestinationScript(this ILogger logger, string script);    
}