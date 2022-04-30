using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostreSQLMsSqlMigrationTool
{
    internal interface ITableReader : IDisposable
    {
        void Open(string tableName, IList<string> colNames);

        bool Read();

        IList<object?> GetValues();
    }
}
