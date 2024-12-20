using DotNet.Testcontainers.Containers;
using LightBDD.XUnit2;
using Microsoft.Extensions.Configuration;
using SncSoftware.Extensions.Dapper.ServiceTests.Infrastructure;
using SncSoftware.Extensions.Dapper.ServiceTests.Infrastructure.Persistence;
using Testcontainers.PostgreSql;

[assembly: ConfiguredLightBddScope]
[assembly: ClassCollectionBehavior(AllowTestParallelization = true)]

namespace SncSoftware.Extensions.Dapper.ServiceTests.Infrastructure;
public class ConfiguredLightBddScopeAttribute : LightBddScopeAttribute
{
    private IContainer _postgresContainer;
    private PostgresDatabaseMigrator _postgresDatabaseMigrator;

    protected override void OnSetUp()
    {
        var postgresSettings = Settings.DatabaseSettings;
        var postgresDatabase = Settings.DatabaseName;
        _postgresDatabaseMigrator = new PostgresDatabaseMigrator(postgresSettings.ConnectionString, postgresDatabase);
        _postgresContainer = new PostgreSqlBuilder()
            .WithImage("postgres:latest")
            .WithPortBinding(5432, 5432)
            .Build();

        _postgresContainer.StartAsync().Wait();
        _postgresDatabaseMigrator.EnsureCreated();

        ServiceCollectionFactory.Initialize();
    }

    protected override void OnTearDown()
    {
        _postgresDatabaseMigrator.EnsureDeleted();
        _postgresContainer.StopAsync().GetAwaiter().GetResult();
    }
}