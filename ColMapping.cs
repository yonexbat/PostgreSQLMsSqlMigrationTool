using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostreSQLMsSqlMigrationTool
{
    internal class ColMapping
    {
        public string? SourceColName { get; set; }

        public string? DestinationColName { get; set; }
    }
}
