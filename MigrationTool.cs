using Microsoft.Extensions.Configuration;
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

        public MigrationTool(IConfiguration configuration)
        {
            _configuration = configuration;
            _sourceConnectionString = configuration.GetConnectionString("SourceDatabase");
            _targetConnectionString = configuration.GetConnectionString("DestinationDatabase");
            _migrationOptions = new MigrationOptions();
            _configuration.GetSection(MigrationOptions.Migration).Bind(_migrationOptions);
        }

        public void Migrate()
        {
            foreach(var migration in _migrationOptions.MigrationItems)
            {
                List<string> colNames = migration.ColMappings.Select(x => x.SourceColName).ToList()!;
                using var tableReader = new MsSqlTableReader(_sourceConnectionString, migration.SourceTableName, colNames);
                tableReader.Open();

                List<string> colNamesDest = migration.ColMappings.Select(x => x.DestinationColName).ToList()!;
                using var tableWriter = new PostgreSqlTableWriter(_targetConnectionString, migration.DestinationTableName, colNamesDest);
                tableWriter.Open();

                while (tableReader.Read())
                {
                    var values = tableReader.GetValues();
                    tableWriter.Write(values);
                }
            }
        }

    }
}
