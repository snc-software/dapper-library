namespace DapperApplication.Tests;

public class AuditDatabaseContext : DatabaseContext
{
    public virtual DatabaseCollection<AuditLog> AuditLogs { get; set; }
}