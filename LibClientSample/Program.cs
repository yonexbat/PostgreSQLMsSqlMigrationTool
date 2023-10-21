// See https://aka.ms/new-console-template for more information

using CopyTableData;

Console.WriteLine("Hello, World!");

var connectionStrings = new ConnectionStrings("Server=localhost;Database=sample;User Id=SA;Password=eXample4eX3+24234$+;",
    "User ID=postgres;Password=example;Host=localhost;Port=5432;Database=postgres;");

var options = new MigrationOptions()
{
    Name = "Migrate employee application",
    SourceDbTech = "mssql",
    DestinationDbTech = "pgsql",
    MigrationItems = new List<MigrationItem>()
    {
        new() 
        {
            SourceTableName = "SampleTable",
            DestinationTableName = "sampletable",
            ColMappings = new List<ColMapping>()
            {
                new()
                {
                    SourceColName = "Id",
                    DestinationColName = "id",
                },
                new()
                {
                    SourceColName = "SomeBit",
                    DestinationColName = "somebit",
                },
                new()
                {
                    SourceColName = "SomeDate",
                    DestinationColName = "somedate",
                },
                new()
                {
                    SourceColName = "SomeTextVarchar",
                    DestinationColName = "sometextvarchar",
                },
                new()
                {
                    SourceColName = "SomeTextNVarchar",
                    DestinationColName = "sometextnvarchar",
                },
                new()
                {
                    SourceColName = "SomeTextChar",
                    DestinationColName = "sometextchar",
                },
                new()
                {
                    SourceColName = "SomeSmallInt",
                    DestinationColName = "somesmallint",
                },
            }
        }
    }
};

CopyDataTable.CopyTables(connectionStrings, options);
    
    