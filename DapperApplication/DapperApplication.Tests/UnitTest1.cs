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

        var saveChanges = databaseContext.SaveChanges(default);
    }
}