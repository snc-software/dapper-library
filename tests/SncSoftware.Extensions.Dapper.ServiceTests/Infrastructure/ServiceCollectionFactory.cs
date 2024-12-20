using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using SncSoftware.Extensions.Dapper.ServiceTests.Contracts;
using SncSoftware.Extensions.Dapper.ServiceTests.Infrastructure.Persistence;

namespace SncSoftware.Extensions.Dapper.ServiceTests.Infrastructure;

public class ServiceCollectionFactory
{
    private static ServiceCollectionFactory s_instance;

    public ServiceCollectionFactory(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
    }

    public IServiceProvider ServiceProvider { get; private set; }

    public static ServiceCollectionFactory Instance
    {
        get
        {
            if (s_instance is not null)
            {
                return s_instance;
            }

            throw new Exception("Instance is not initialized");
        }
    }

    public static void Initialize()
    {
        var services = new ServiceCollection();
        services.AddDatabaseContext<TestDatabaseContext>(Settings.Configuration);
        var serviceProvider = services.BuildServiceProvider();
        s_instance = new ServiceCollectionFactory(serviceProvider);
    }
    
    /// <summary>
    /// To be used ONLY with Get All calls to avoid test spillage
    /// </summary>
    /// <param name="databaseName">name of database</param>
    /// <returns></returns>
    public static IServiceProvider GetWithDatabase(string databaseName)
    {
        var postgresSettings = Settings.DatabaseSettings;
        var builder = new ConfigurationBuilder();
        var connectionStringBuilder = new NpgsqlConnectionStringBuilder(postgresSettings.ConnectionString)
        {
            Database = databaseName
        };
        var configurationValues = new Dictionary<string, string>
        {
            { "Postgres:ConnectionString", connectionStringBuilder.ToString() }
        };
        var configuration = builder.AddInMemoryCollection(configurationValues!).Build();
        
        var services = new ServiceCollection();
        services.AddDatabaseContext<TestDatabaseContext>(configuration);
        return services.BuildServiceProvider();
    }
}