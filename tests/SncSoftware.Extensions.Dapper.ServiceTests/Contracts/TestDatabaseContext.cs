namespace SncSoftware.Extensions.Dapper.ServiceTests.Contracts;

public class TestDatabaseContext : DatabaseContext
{
    public virtual DatabaseCollection<TestEntity> TestEntities { get; set; }
}