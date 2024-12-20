using AutoFixture;
using FluentAssertions;
using LightBDD.XUnit2;
using Microsoft.Extensions.DependencyInjection;
using SncSoftware.Extensions.Dapper.ServiceTests.Contracts;
using SncSoftware.Extensions.Dapper.ServiceTests.Infrastructure;
using SncSoftware.Extensions.Dapper.ServiceTests.Infrastructure.Persistence;

namespace SncSoftware.Extensions.Dapper.ServiceTests.Features.AttributeEntities;

public partial class Get_All_Feature : FeatureFixture, IAsyncLifetime
{
    private TestDatabaseContext DatabaseContext;
    private PostgresDatabaseMigrator _postgresDatabaseMigrator;

    private readonly List<EntityWithAttributes> _entities;
    private List<EntityWithAttributes> _retrievedEntities;
    private readonly string _databaseName = $"get-all-{Guid.NewGuid()}";

    public Get_All_Feature()
    {
        var fixture = new Fixture();
        var entitiesToCreate = Random.Shared.Next(5, 20);
        
        _entities = fixture.CreateMany<EntityWithAttributes>(entitiesToCreate)
            .ToList();
    }

    private async Task Entities_exists_in_database()
    {
        foreach (var entity in _entities)
        {
            await EntityWithAttributesPostgresProvider.Insert(entity, _databaseName);
        }
    }

    private async Task Get_all_is_requested()
    {
        _retrievedEntities = (await DatabaseContext.TestEntities.GetAll()).ToList();
    }

    private Task Retrieved_entities_match_source()
    {
        _retrievedEntities.Should().BeEquivalentTo(_entities);

        return Task.CompletedTask;
    }

    public Task InitializeAsync()
    {
        var databaseSettings = Settings.DatabaseSettings;
        _postgresDatabaseMigrator = new PostgresDatabaseMigrator(databaseSettings.ConnectionString, _databaseName);
        _postgresDatabaseMigrator.EnsureCreated();
        
        var serviceProvider = ServiceCollectionFactory.GetWithDatabase(_databaseName);
        DatabaseContext = serviceProvider.GetRequiredService<TestDatabaseContext>();

        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        _postgresDatabaseMigrator.EnsureDeleted();
        return Task.CompletedTask;
    }
}