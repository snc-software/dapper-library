namespace DapperApplication.Tests.Models;

public class AuditDatabaseContext : DatabaseContext
{
    public virtual DatabaseCollection<AuditLog> AuditLogs { get; set; }
    public virtual DatabaseCollection<TypedAuditLog> TypedAuditLogs { get; set; }
}