using Microsoft.Extensions.Configuration;
using Npgsql;

namespace SncSoftware.Extensions.Dapper.ServiceTests.Infrastructure;

public static class Settings
{
    public static readonly string DatabaseName = $"dapper-library-{Guid.NewGuid()}";
    public static DatabaseSettings DatabaseSettings { get; private set; }
    public static IConfiguration Configuration { get; private set; }

    static Settings()
    {
        var builder = new ConfigurationBuilder();
        const string CoreConnectionString = "Server=localhost;Port=5432;Database=postgres;User Id=postgres;Password=postgres;";
        var connectionStringBuilder = new NpgsqlConnectionStringBuilder(CoreConnectionString)
        {
            Database = DatabaseName
        };

        var configurationValues = new Dictionary<string, string>()
        {
            { "Postgres:ConnectionString", connectionStringBuilder.ToString() }
        };
        var configuration = builder.AddInMemoryCollection(configurationValues!).Build();
        Configuration = configuration;
        DatabaseSettings = new DatabaseSettings
        {
            ConnectionString = connectionStringBuilder.ToString()
        };
    }
}