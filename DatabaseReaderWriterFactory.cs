using Microsoft.Extensions.DependencyInjection;
using PostreSQLMsSqlMigrationTool.MsSql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostreSQLMsSqlMigrationTool
{
    internal class DatabaseReaderWriterFactory
    {
        private readonly IServiceProvider _serviceProvider;
        public DatabaseReaderWriterFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }


        public ITableReader CreateTableReader(string tech)
        {
            ITableReader? reader = null;

            switch (tech)
            {
                case "mssql":
                    reader = _serviceProvider.GetService<MsSqlTableReader>();
                    break;
                
            }

            if (reader == null) {
                throw new NotSupportedException($"{tech} not supported.");
            }

            return reader;

        }
    }
}
