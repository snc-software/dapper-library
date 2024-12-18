namespace DapperApplication.Tests;

public interface IAuditLogRetrievalService
{
    Task<AuditLog> GetAudit(Guid id, CancellationToken cancellationToken);
}

public class AuditLogRetrievalService : IAuditLogRetrievalService
{
    private readonly AuditDatabaseContext _context;

    public AuditLogRetrievalService(AuditDatabaseContext context)
    {
        _context = context;
    }

    public async Task<AuditLog> GetAudit(Guid id, CancellationToken cancellationToken)
    {
        return await _context.AuditLogs.GetById(p => p.Id, id, cancellationToken);
    }
}