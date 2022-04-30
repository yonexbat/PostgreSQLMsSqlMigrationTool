using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PostreSQLMsSqlMigrationTool;
using PostreSQLMsSqlMigrationTool.MsSql;

Startup(args);
return 0;

static void Startup(string[] args)
{
    
    var loggerFactory = LoggerFactory.Create(loggerBuilder => loggerBuilder.AddConsole());
    var configuration = GetConfiguration(args);


    //setup DI
    var serviceProvider = new ServiceCollection()
        .AddLogging()
        .AddSingleton<MigrationTool>()
        .AddSingleton<DatabaseReaderWriterFactory>()
        .AddSingleton(provider => configuration)
        .AddSingleton(loggerFactory)
        .AddTransient<MsSqlTableReader>()
        .BuildServiceProvider();

    MigrationTool? tool = serviceProvider.GetService<MigrationTool>();
    if (tool == null)
    {
        throw new InvalidOperationException("Migrationtool ist not registered in di");
    }
    tool.Migrate();
}



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







