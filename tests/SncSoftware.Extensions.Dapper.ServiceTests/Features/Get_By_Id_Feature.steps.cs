using FluentAssertions;
using LightBDD.XUnit2;
using Microsoft.Extensions.DependencyInjection;
using SncSoftware.Extensions.Dapper.ServiceTests.Contracts;
using SncSoftware.Extensions.Dapper.ServiceTests.Infrastructure;
using SncSoftware.Extensions.Dapper.ServiceTests.Infrastructure.Persistence;

namespace SncSoftware.Extensions.Dapper.ServiceTests.Features;

public partial class Get_By_Id_Feature : FeatureFixture
{
    private static TestDatabaseContext _databaseContext =
        ServiceCollectionFactory.Instance.ServiceProvider.GetRequiredService<TestDatabaseContext>();

    private readonly TestEntity _entity;
    private TestEntity _retrievedEntity;

    public Get_By_Id_Feature()
    {
        _entity = new TestEntity
        {
            Id = Guid.NewGuid(),
            Age = Random.Shared.Next(18, 60),
            Description = $"Description{Guid.NewGuid()}",
            Enabled = Random.Shared.Next(100) < 50
        };
    }

    private async Task Entity_exists_in_database()
    {
        await TestEntityPostgresProvider.Insert(_entity);
    }

    private async Task Get_By_Id_is_requested()
    {
        _retrievedEntity = await _databaseContext.TestEntities.GetById(_entity.Id);
    }

    private Task Retrieved_entity_should_match_source()
    {
        _retrievedEntity.Should().BeEquivalentTo(_entity);

        return Task.CompletedTask;
    }
}