using CopyTableData;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

Startup(args);
return 0;

static void Startup(string[] args)
{
    var loggerFactory = LoggerFactory.Create(loggerBuilder => loggerBuilder.AddConsole());
    var configuration = GetConfiguration(args);
    var connectionStringSource = configuration.GetConnectionString("SourceDatabase");
    var connectionStringDestination = configuration.GetConnectionString("DestinationDatabase");
    var connectionStrings = new ConnectionStrings(connectionStringSource!, connectionStringDestination!);
    var migrationOptions = new MigrationOptions();
    configuration.GetSection(MigrationOptions.Migration).Bind(migrationOptions);


    //setup DI
    var serviceProvider = new ServiceCollection()
        .AddLogging()
        .AddSingleton<MigrationTool>()
        .AddSingleton(migrationOptions)
        .AddSingleton(connectionStrings)
        .AddSingleton<DatabaseReaderWriterFactory>()
        .AddSingleton(provider => configuration)
        .AddSingleton(loggerFactory)
        .BuildServiceProvider();

    var tool = serviceProvider.GetService<MigrationTool>();
    if (tool == null) throw new InvalidOperationException("Migrationtool ist not registered in di");
    tool.Migrate();
}


static IConfiguration GetConfiguration(string[] args)
{
    var configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json")
        .AddEnvironmentVariables()
        .AddCommandLine(args)
        .AddUserSecrets<Program>()
        .Build();

    return configuration;
}