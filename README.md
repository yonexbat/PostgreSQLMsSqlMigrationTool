# PostreSQLMsSqlMigrationTool
Another data migration tool.
The following mappings work from mssql to postgresql. 

|MSSQL DATATYPE|POSTGRESQL DATATYPE|
|--------------|-------------------|
|int|integer|
|datetimeoffset(7)|timestamp with timezone|
|datetimeoffset(7)|timestamp without timezone|
|nvarchar(max)|character varying(255)|
