using DapperApplication.Attributes;

namespace DapperApplication.Tests;

[TableName("Logs")]
public class TypedAuditLog
{
    [PrimaryIdentifier]
    public Guid Id { get; set; }
    public string Body { get; set; }
}