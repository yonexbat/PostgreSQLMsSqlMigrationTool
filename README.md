# PostgreSQLMsSqlMigrationTool

Another data migration tool. Migration from *mssql* to *postresql* and from *postresql* to *mssql* is
supported.

The following mappings work from mssql to postgresql. Other mappings will probably work too, but are not tested.

| MSSQL DATATYPE    | POSTGRESQL DATATYPE        |
|-------------------|----------------------------|
| int               | integer                    |
| datetimeoffset(7) | timestamp with timezone    |
| datetimeoffset(7) | timestamp without timezone |
| datetimeoffset(7) | date                       |
| nvarchar(*)       | character varying(*)       |
| varchar(*)        | character varying(*)       |
| char(*)           | character varying(*)       |
| varchar(*)        | text                       |
| smallint          | smallint                   |
| bit               | boolean                    |
| varbinary         | bytea                      |

## Getting started

### Step 1

Clone this repository and open it with an *ide* like viusal studio or rider.

### Step 2

Create table in source database (mssql). Just for demonstration purposes. You probably have already tables and data in
the source database.

    CREATE TABLE [dbo].[SampleTable]
    (
        [Id] INT NOT NULL PRIMARY KEY,
        [SomeBit] BIT NULL, 
        [SomeDate] DATETIME2 NULL, 
        [SomeTextVarchar] VARCHAR(50) NULL, 
        [SomeTextNVarchar] NVARCHAR(50) NULL, 
        [SomeTextChar] CHAR(10) NULL, 
        [SomeSmallInt] SMALLINT NULL, 
    )

Add some data to it.

    INSERT INTO dbo.SampleTable
    (
        [Id],
        [SomeBit],
        [SomeDate],
        [SomeTextVarchar],
        [SomeTextNVarchar],
        [SomeTextChar],
        [SomeSmallInt]
    )
    VALUES
    (
        1,
        1,
        '2022-04-30',
        'HELLO',
        'World',
        '123abc',
        5
    ),
    (
        2,
        0,
        '2025-04-30',
        'HELLO',
        'World',
        '123abc',
        5
    )

### Step 3

Create simple intermediate tables in PostgreSQL. The intermediate tables should be more or less identical to the source
tables.

    DROP TABLE IF EXISTS public.sampletable;

    CREATE TABLE IF NOT EXISTS public.sampletable
    (
        id integer,
        somebit boolean,
        somedate timestamp with time zone,
        sometextvarchar character varying(255) COLLATE pg_catalog."default",
        sometextnvarchar character varying(255) COLLATE pg_catalog."default",
        sometextchar character varying(10) COLLATE pg_catalog."default",
        somesmallint smallint
    )

### Step 4

Go to project (folder) *Application*. Open file *appsettings.json* and set connection strings for SourceDatabase and DestinationDatabase. Then add a
*migration-item* for each table you want to migrate. If the col-mappings are not set, the tool will try to map the columns. The names have to be identical (case-insensitive). 
The order of the columns is not important. The tool will try to map the columns by name. If the column names are different, you have to set the col-mappings.

    {
      "ConnectionStrings": {
        "SourceDatabase": "Server=tcp:someval.database.windows.net,1433;Initial Catalog=somedb;Persist Security Info=False;User ID=someuser;Password=SomePW;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;",
        "DestinationDatabase": "Host=auroraclustername.cluster-something.eu-central-1.rds.amazonaws.com;Username=postgres;Password=something;Database=somedb"
      },

      "Migration": {
        "Name": "Migrate employee application",
        "SourceDbTech": "mssql",
        "DestinationDbTech":  "pgsql",
        "MigrationItems": [
          {
            "SourceTableName": "SampleTable",
            "DestinationTableName": "sampletable"
          }
        ]
      }
    }

> Note: add connection strings to user-secrets.

Here a sample with col-mappings:

    {
      "ConnectionStrings": {
        "SourceDatabase": "Server=tcp:someval.database.windows.net,1433;Initial Catalog=somedb;Persist Security Info=False;User ID=someuser;Password=SomePW;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;",
        "DestinationDatabase": "Host=auroraclustername.cluster-something.eu-central-1.rds.amazonaws.com;Username=postgres;Password=something;Database=somedb"
      },

      "Migration": {
        "Name": "Migrate employee application",
        "SourceDbTech": "mssql",
        "DestinationDbTech":  "pgsql",
        "MigrationItems": [
          {
            "SourceTableName": "SampleTable",
            "DestinationTableName": "sampletable",
            "ColMappings": [
              {
                "SourceColName": "Id",
                "DestinationColName": "id"
              },          
              {
                "SourceColName": "SomeBit",
                "DestinationColName": "somebit"
              },
              {
                "SourceColName": "SomeDate",
                "DestinationColName": "somedate"
              },
              {
                "SourceColName": "SomeTextVarchar",
                "DestinationColName": "sometextvarchar"
              },
              {
                "SourceColName": "SomeTextNVarchar",
                "DestinationColName": "sometextnvarchar"
              },
              {
                "SourceColName": "SomeTextChar",
                "DestinationColName": "sometextchar"
              },
              {
                "SourceColName": "SomeSmallInt",
                "DestinationColName": "somesmallint"
              }
            ]
          }
        ]
      }
    }

### Step 5

Start the application.

### Step 6

Create sql statements to transfer data from the intermediate tables to the final tables.
