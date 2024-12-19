using AutoFixture;
using FluentAssertions;
using SncSoftware.Extensions.Dapper.Providers;

namespace SncSoftware.Extensions.Dapper.UnitTests.Providers;

public class ExecuteQueryProviderTests
{
    private readonly IExecuteQueryProvider _provider = new ExecuteQueryProvider();
    private readonly IFixture _fixture = new Fixture();

    [Fact]
    public void GetQueries_Returns_Empty_Array_With_No_Queries()
    {
        var queries = _provider.GetQueries();

        queries.Should().BeEmpty();
    }

    [Fact]
    public void Queries_Can_Be_Added_And_Retrieved()
    {
        var queries = _fixture.CreateMany<DatabaseQuery>().ToList();
        foreach (var query in queries)
        {
            _provider.AddQuery(query);
        }

        var retrievedQueries = _provider.GetQueries().ToList();

        retrievedQueries.Should().NotBeNullOrEmpty();
        retrievedQueries.Should().BeEquivalentTo(queries);
    }

    [Fact]
    public void Clear_Removes_All_Queries()
    {
        var queries = _fixture.CreateMany<DatabaseQuery>().ToList();
        foreach (var query in queries)
        {
            _provider.AddQuery(query);
        }

        var retrievedQueries = _provider.GetQueries().ToList();

        retrievedQueries.Should().NotBeNullOrEmpty();
        retrievedQueries.Should().BeEquivalentTo(queries);

        _provider.Clear();
        retrievedQueries = _provider.GetQueries().ToList();

        retrievedQueries.Should().BeEmpty();
    }
}