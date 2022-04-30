using Microsoft.Extensions.Configuration;
using PostreSQLMsSqlMigrationTool;

Console.WriteLine("starting up migration tool");
var configuration = GetConfiguration(args);
MigrationTool tool = new MigrationTool(configuration);
tool.Migrate();
return 0;


static IConfiguration GetConfiguration(string[] args)
{
    var configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile($"appsettings.json")
        .AddEnvironmentVariables()
        .AddCommandLine(args)
        .AddUserSecrets<Program>()
        .Build();

    return configuration;
}







