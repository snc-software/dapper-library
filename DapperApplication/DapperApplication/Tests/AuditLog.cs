namespace DapperApplication.Tests;

public record AuditLog
{
    public Guid Id { get; set; }
    public string Body { get; set; }
}