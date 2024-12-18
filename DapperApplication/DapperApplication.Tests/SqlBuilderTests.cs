using DapperApplication.SqlBuilders;

namespace DapperApplication.Tests;

public class SqlBuilderTests
{
    [Fact]
    public void SelectProperties()
    {
        var sqlBuilder = new SqlBuilder<TestEntity>()
            .Select()
            .Property(p => p.Description)
            .Property(p => p.Age)
            .FromTable("Tests")
            .Where(where => where
                    .PropertyMatches(p => p.Id, Guid.NewGuid().ToString())
                    .And()
                    .PropertyMatches(p => p.Description, "Bananas"));
        var databaseQuery = sqlBuilder.BuildQuery();
    }
    
    [Fact]
    public void SelectAll()
    {
        var sqlBuilder = new SqlBuilder<TestEntity>()
            .Select()
            .AllProperties()
            .FromTable("Tests")
            .Where(where => where
                .PropertyMatches(p => p.Id, Guid.NewGuid().ToString())
                .And()
                .PropertyMatches(p => p.Description, "Bananas"));
        var databaseQuery = sqlBuilder.BuildQuery();
    }
}


public class TestEntity
{
    public Guid Id { get; set; }
    public string Description { get; set; }
    public int Age { get; set; }
}