namespace DapperApplication.Tests;

public interface IAuditLogRetrievalService
{
    Task<AuditLog> GetAudit(Guid id);
}

public class AuditLogRetrievalService : IAuditLogRetrievalService
{
    private readonly AuditDatabaseContext _context;

    public AuditLogRetrievalService(AuditDatabaseContext context)
    {
        _context = context;
    }

    public async Task<AuditLog> GetAudit(Guid id)
    {
        return await _context.AuditLogs.GetById(id);
    }
}