# PostreSQLMsSqlMigrationTool

Another data migration tool. Currently migration from *mssql* to *postresql* is supported.

The following mappings work from mssql to postgresql.

|MSSQL DATATYPE|POSTGRESQL DATATYPE|
|--------------|-------------------|
|int|integer|
|datetimeoffset(7)|timestamp with timezone|
|datetimeoffset(7)|timestamp without timezone|
|nvarchar(255)|character varying(255)|
|varchar(255)|character varying(255)|
|char(10)|character varying(10)|
|smallint|smallint|

## Getting started

### Step 1

Clone this repository and open it with an ide like viusal studio. net 6 or higher required.

### Step 2

Create table in source database (mssql). Just for demonstartion purposes. You probably have already tables and data in the source database.

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

Create simple intermediate tables in PostgreSQL. Keep the intermediate tables as simple as possible.

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

    TABLESPACE pg_default;

    ALTER TABLE IF EXISTS public.sampletable
        OWNER to postgres;

### Step 4

Open file appsettings.json and set connection strings for SourceDatabase and DestinationDatabase. Then add a *migration-item* for each table you want to migrate.

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
Create sql statements to transfer to data from the intermediate tables to the final tables.
