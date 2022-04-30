using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostreSQLMsSqlMigrationTool
{
    internal interface ITableWriter: IDisposable
    {
        void Open(string tableName, IList<string> colNames);

        public void Write(object?[] values);
    }
}
