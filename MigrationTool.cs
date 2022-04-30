using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace PostreSQLMsSqlMigrationTool
{
    internal class MigrationTool
    {
        private IConfiguration _configuration;
        private MigrationOptions _migrationOptions;
        private ILogger _logger;
        private DatabaseReaderWriterFactory _databaseReaderWriterFactory;

        public MigrationTool(IConfiguration configuration, 
            DatabaseReaderWriterFactory databaseReaderWriterFactory,
            ILogger<MigrationTool> logger)
        {
            _configuration = configuration;
            _migrationOptions = new MigrationOptions();
            _configuration.GetSection(MigrationOptions.Migration).Bind(_migrationOptions);
            _databaseReaderWriterFactory = databaseReaderWriterFactory;
            _logger = logger;
        }

        public void Migrate()
        {
            foreach(var migration in _migrationOptions.MigrationItems)
            {
                MigrateTable(migration);               
            }
        }

        private void MigrateTable(MigrationItem migration)
        {
            Log.StartMigrationItem(_logger, migration, default!);

            using var tableReader = OpenNewTableReader(migration);
            using var tableWriter = OpenNewTableWriter(migration);

            int counter = 0;

            while (tableReader.Read())
            {
                counter++;
                var values = tableReader.GetValues();
                tableWriter.Write(values);
                if(counter % 100 == 0)
                {
                    Log.CountInfo(_logger, counter, default!);
                }
            }
            Log.CountInfo(_logger, counter, default!);
        }

        private ITableReader OpenNewTableReader(MigrationItem migration)
        {
            List<string> colNames = migration.ColMappings.Select(x => x.SourceColName).ToList()!;
            var reader = _databaseReaderWriterFactory.CreateTableReader(_migrationOptions.SourceDbTech);
            reader.Open(migration.SourceTableName, colNames);
            return reader;
        }

        private ITableWriter OpenNewTableWriter(MigrationItem migration)
        {
            List<string> colNamesDest = migration.ColMappings.Select(x => x.DestinationColName).ToList()!;
            var writer =  _databaseReaderWriterFactory.CreateTableWriter(_migrationOptions.DestinationDbTech);
            writer.Open(migration.DestinationTableName, colNamesDest);
            return writer;
        }

        internal class Log
        {
            static internal readonly Action<ILogger, MigrationItem, Exception> StartMigrationItem = LoggerMessage.Define<MigrationItem>(
              LogLevel.Information,
              new EventId(1, "Starting migration"),
              "Starting migration {Item}");

            static internal readonly Action<ILogger, int, Exception> CountInfo = LoggerMessage.Define<int>(
              LogLevel.Information,
              new EventId(2, "Count Info"),
              "Migrated {numrows} rows");
        }
    }
}
