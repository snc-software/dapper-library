namespace DapperApplication.Tests;

public class AuditDatabaseContext : DatabaseContext
{
    public AuditDatabaseContext(DbSettings settings) : base(settings)
    {
    }
    
    public virtual DatabaseCollection<AuditLog> AuditLogs { get; set; }
}