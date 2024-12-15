// See https://aka.ms/new-console-template for more information

using CopyTableData;

var connectionStrings = new ConnectionStrings("Server=localhost;Database=sample;User Id=SA;Password=eXample4eX3+24234$+;",
    "User ID=postgres;Password=example;Host=localhost;Port=5432;Database=postgres;");

var options = new MigrationOptions
{
    Name = "Migrate employee application",
    SourceDbTech = "mssql",
    DestinationDbTech = "pgsql",
    MigrationItems = new List<MigrationItem>
    {
        new()
        {
            SourceTableName = "SampleTable",
            DestinationTableName = "sampletable",
        },
    },
};

CopyDataTable.CopyTables(connectionStrings, options);