namespace DapperApplication.Tests.Models;

public record AuditLog
{
    public Guid Id { get; set; }
    public string Body { get; set; }
}