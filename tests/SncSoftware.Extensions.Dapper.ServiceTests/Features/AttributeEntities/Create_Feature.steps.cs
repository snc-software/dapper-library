using AutoFixture;
using FluentAssertions;
using LightBDD.XUnit2;
using Microsoft.Extensions.DependencyInjection;
using SncSoftware.Extensions.Dapper.ServiceTests.Contracts;
using SncSoftware.Extensions.Dapper.ServiceTests.Infrastructure;
using SncSoftware.Extensions.Dapper.ServiceTests.Infrastructure.Persistence;

namespace SncSoftware.Extensions.Dapper.ServiceTests.Features.AttributeEntities;

public partial class Create_Feature : FeatureFixture
{
    private static readonly TestDatabaseContext DatabaseContext =
        ServiceCollectionFactory.Instance.ServiceProvider.GetRequiredService<TestDatabaseContext>();

    private readonly EntityWithAttributes _entity;

    public Create_Feature()
    {
        var fixture = new Fixture();
        _entity = fixture.Create<EntityWithAttributes>();
    }

    private async Task Create_is_requested_and_saved()
    {
        DatabaseContext.TestEntities.Create(_entity);
        await DatabaseContext.SaveChanges(CancellationToken.None);
    }

    private async Task Entity_is_created_in_the_database()
    {
        var retrievedEntity = await EntityWithAttributesPostgresProvider.Get(_entity.Id);

        retrievedEntity.Should().NotBeNull();
        retrievedEntity.Should().BeEquivalentTo(_entity);
    }

}