using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PostreSQLMsSqlMigrationTool.MsSql;
using PostreSQLMsSqlMigrationTool.PostgreSql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostreSQLMsSqlMigrationTool
{
    internal class MigrationTool
    {
        private IConfiguration _configuration;
        private string _sourceConnectionString;
        private string _targetConnectionString;
        private MigrationOptions _migrationOptions;
        private ILogger _logger;
        private DatabaseReaderWriterFactory _databaseReaderWriterFactory;

        public MigrationTool(IConfiguration configuration, 
            DatabaseReaderWriterFactory databaseReaderWriterFactory,
            ILogger<MigrationTool> logger)
        {
            _configuration = configuration;
            _sourceConnectionString = configuration.GetConnectionString("SourceDatabase");
            _targetConnectionString = configuration.GetConnectionString("DestinationDatabase");
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
            Log.InformationLoggerMessageOneParam(_logger, migration, default!);
            using var tableReader = CreateTableReader();

            List<string> colNames = migration.ColMappings.Select(x => x.SourceColName).ToList()!;
            tableReader.Open(migration.SourceTableName, colNames);

            List<string> colNamesDest = migration.ColMappings.Select(x => x.DestinationColName).ToList()!;

            using var tableWriter = CreateTableWriter();
            tableWriter.Open(migration.DestinationTableName, colNamesDest);

            while (tableReader.Read())
            {
                var values = tableReader.GetValues();
                tableWriter.Write(values);
            }
        }

        private ITableReader CreateTableReader()
        {
            return _databaseReaderWriterFactory.CreateTableReader(_migrationOptions.SourceDbTech);
        }

        private ITableWriter CreateTableWriter()
        {
            return _databaseReaderWriterFactory.CreateTableWriter(_migrationOptions.DestinationDbTech);
        }

        internal class Log
        {
            static internal readonly Action<ILogger, MigrationItem, Exception> InformationLoggerMessageOneParam = LoggerMessage.Define<MigrationItem>(
              LogLevel.Information,
              new EventId(1, "Starting migration"),
              "Starting migration {Item}");
        }
    }
}
