using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostreSQLMsSqlMigrationTool
{
    internal class MigrationOptions
    {
        public const string Migration = "Migration";

        public string Name { get; set; } = string.Empty;

        public IList<MigrationItem> MigrationItems { get; set; } = new List<MigrationItem>();

    }
}
