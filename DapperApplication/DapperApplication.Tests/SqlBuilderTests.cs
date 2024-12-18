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

    [Fact]
    public void Insert()
    {
        var sqlBuilder = new SqlBuilder<TestEntity>()
            .Insert()
            .IntoTable("Tests")
            .Value(p => p.Id, Guid.NewGuid().ToString())
            .Value(p => p.Description, "Bananas")
            .Value(p => p.Age, 10.ToString());
        
        var databaseQuery = sqlBuilder.BuildQuery();
    }
    
    [Fact]
    public void Update()
    {
        var sqlBuilder = new SqlBuilder<TestEntity>()
            .Update()
            .Table("Tests")
            .SetValue(p => p.Id, Guid.NewGuid().ToString())
            .SetValue(p => p.Description, "Bananas")
            .SetValue(p => p.Age, 10.ToString())
            .Where(w => w.PropertyMatches(p => p.Id, Guid.NewGuid().ToString()));
        
        var databaseQuery = sqlBuilder.BuildQuery();
    }
}


public class TestEntity
{
    public Guid Id { get; set; }
    public string Description { get; set; }
    public int Age { get; set; }
}