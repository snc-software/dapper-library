using AutoFixture;
using FluentAssertions;
using LightBDD.XUnit2;
using Microsoft.Extensions.DependencyInjection;
using SncSoftware.Extensions.Dapper.ServiceTests.Contracts;
using SncSoftware.Extensions.Dapper.ServiceTests.Infrastructure;
using SncSoftware.Extensions.Dapper.ServiceTests.Infrastructure.Persistence;

namespace SncSoftware.Extensions.Dapper.ServiceTests.Features.AttributeEntities;

public partial class Delete_By_Id_Feature : FeatureFixture
{
    private static readonly TestDatabaseContext DatabaseContext =
        ServiceCollectionFactory.Instance.ServiceProvider.GetRequiredService<TestDatabaseContext>();

    private readonly EntityWithAttributes _entity;

    public Delete_By_Id_Feature()
    {
        var fixture = new Fixture();
        _entity = fixture.Create<EntityWithAttributes>();
    }

    private async Task Entity_exists_in_database()
    {
        await EntityWithAttributesPostgresProvider.Insert(_entity);
        var entity = EntityWithAttributesPostgresProvider.Get(_entity.Id);
        entity.Should().NotBeNull();
    }

    private async Task Delete_is_requested_and_saved()
    {
        DatabaseContext.TestEntities.DeleteById(_entity.Id);
        await DatabaseContext.SaveChanges(CancellationToken.None);
    }

    private async Task Entity_is_deleted()
    {
        var entity = await EntityWithAttributesPostgresProvider.Get(_entity.Id);
        entity.Should().BeNull();
    }
}