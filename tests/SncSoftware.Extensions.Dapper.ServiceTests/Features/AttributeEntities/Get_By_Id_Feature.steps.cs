using AutoFixture;
using FluentAssertions;
using LightBDD.XUnit2;
using Microsoft.Extensions.DependencyInjection;
using SncSoftware.Extensions.Dapper.ServiceTests.Contracts;
using SncSoftware.Extensions.Dapper.ServiceTests.Infrastructure;
using SncSoftware.Extensions.Dapper.ServiceTests.Infrastructure.Persistence;

namespace SncSoftware.Extensions.Dapper.ServiceTests.Features.AttributeEntities;

public partial class Get_By_Id_Feature : FeatureFixture
{
    private static readonly TestDatabaseContext DatabaseContext =
        ServiceCollectionFactory.Instance.ServiceProvider.GetRequiredService<TestDatabaseContext>();

    private readonly EntityWithAttributes _entity;
    private EntityWithAttributes _retrievedEntity;

    public Get_By_Id_Feature()
    {
        var fixture = new Fixture();
        _entity = fixture.Create<EntityWithAttributes>();
    }

    private async Task Entity_exists_in_database()
    {
        await EntityWithAttributesPostgresProvider.Insert(_entity);
    }

    private async Task Get_By_Id_is_requested()
    {
        _retrievedEntity = await DatabaseContext.TestEntities.GetById(_entity.Id);
    }

    private Task Retrieved_entity_should_match_source()
    {
        _retrievedEntity.Should().BeEquivalentTo(_entity);

        return Task.CompletedTask;
    }
}