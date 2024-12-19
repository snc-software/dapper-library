using DapperApplication.Tests.Models;
using Microsoft.Extensions.DependencyInjection;

namespace DapperApplication.Tests;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        var serviceProvider = new ServiceCollection()
            .AddDatabaseContext<AuditDatabaseContext>()
            .BuildServiceProvider();
        
        var databaseContext = serviceProvider.GetRequiredService<AuditDatabaseContext>();

        var auditLog = new AuditLog
        {
            Id = Guid.NewGuid(),
            Body = "The body"
        };

        var typedAuditLog = new TypedAuditLog
        {
            Id = auditLog.Id,
            Body = auditLog.Body
        };
        
        databaseContext.AuditLogs.DeleteById(p => p.Id, auditLog.Id);
        databaseContext.TypedAuditLogs.Create(typedAuditLog);
        
        var saveChanges = databaseContext.SaveChanges(default);
    }
}