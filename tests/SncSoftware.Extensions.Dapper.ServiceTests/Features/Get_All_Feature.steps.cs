using AutoFixture;
using FluentAssertions;
using LightBDD.XUnit2;
using Microsoft.Extensions.DependencyInjection;
using SncSoftware.Extensions.Dapper.ServiceTests.Contracts;
using SncSoftware.Extensions.Dapper.ServiceTests.Infrastructure;
using SncSoftware.Extensions.Dapper.ServiceTests.Infrastructure.Persistence;

namespace SncSoftware.Extensions.Dapper.ServiceTests.Features;

public partial class Get_All_Feature : FeatureFixture
{
    private static readonly TestDatabaseContext DatabaseContext =
        ServiceCollectionFactory.Instance.ServiceProvider.GetRequiredService<TestDatabaseContext>();

    private readonly List<TestEntity> _entities;
    private List<TestEntity> _retrievedEntities;

    public Get_All_Feature()
    {
        var fixture = new Fixture();
        var entitiesToCreate = Random.Shared.Next(5, 20);
        _entities = fixture.CreateMany<TestEntity>(entitiesToCreate)
            .ToList();
    }

    private async Task Entities_exists_in_database()
    {
        foreach (var entity in _entities)
        {
            await TestEntityPostgresProvider.Insert(entity);
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
}