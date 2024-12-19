using DapperApplication.Attributes;

namespace DapperApplication.Tests.Models;

[TableName("Logs")]
public class TypedAuditLog
{
    [PrimaryIdentifier]
    public Guid Id { get; set; }
    public string Body { get; set; }
}