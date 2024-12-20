using AutoFixture;
using FluentAssertions;
using LightBDD.XUnit2;
using Microsoft.Extensions.DependencyInjection;
using SncSoftware.Extensions.Dapper.ServiceTests.Contracts;
using SncSoftware.Extensions.Dapper.ServiceTests.Infrastructure;
using SncSoftware.Extensions.Dapper.ServiceTests.Infrastructure.Persistence;

namespace SncSoftware.Extensions.Dapper.ServiceTests.Features.AttributeEntities;

public partial class Update_Feature : FeatureFixture
{
    private static readonly TestDatabaseContext DatabaseContext =
        ServiceCollectionFactory.Instance.ServiceProvider.GetRequiredService<TestDatabaseContext>();

    private readonly EntityWithAttributes _entity;
    private EntityWithAttributes _updatedEntity;

    public Update_Feature()
    {
        var fixture = new Fixture();
        _entity = fixture.Create<EntityWithAttributes>();
    }

    private async Task Entity_exists_in_the_database()
    {
        await EntityWithAttributesPostgresProvider.Insert(_entity);

        var entity = await EntityWithAttributesPostgresProvider.Get(_entity.Id);
        entity.Should().NotBeNull();
        entity.Should().BeEquivalentTo(_entity);
    }

    private async Task Update_is_requested_and_saved()
    {
        _updatedEntity = _entity with
        {
            Description = "Updated description",
            Age = 69,
            Enabled = !_entity.Enabled
        };

        DatabaseContext.TestEntities.Update(_updatedEntity);
        await DatabaseContext.SaveChanges(CancellationToken.None);
    }

    private async Task Updated_entity_is_saved_in_database()
    {
        var entities = (await EntityWithAttributesPostgresProvider.GetAll()).ToList();
        entities.Should().NotBeNullOrEmpty();
        var entitiesForId = entities.Where(e => e.Id == _updatedEntity.Id).ToList();
        entitiesForId.Count.Should().Be(1);
        var entity = entitiesForId.First();
        entity.Should().BeEquivalentTo(_updatedEntity);
    }
}