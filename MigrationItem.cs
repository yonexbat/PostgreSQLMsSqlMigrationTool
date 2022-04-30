using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostreSQLMsSqlMigrationTool
{
    internal class MigrationItem
    {
        public string SourceTableName { get; set; } = String.Empty;

        public string DestinationTableName { get; set; } = String.Empty;

        public IList<ColMapping> ColMappings { get; set; } = new List<ColMapping>();

    }
}
