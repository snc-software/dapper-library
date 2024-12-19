using SncSoftware.Extensions.Dapper.Attributes;

namespace SncSoftware.Extensions.Dapper.ServiceTests.Contracts;

[TableName("TestEntities")]
public record TestEntity
{
    [PrimaryIdentifier]
    public Guid Id { get; set; }
    public string Description { get; set; }
    public int Age { get; set; }
    public bool Enabled { get; set; }
}