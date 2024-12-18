using NSubstitute;

namespace DapperApplication.Tests;

public class DatabaseCollectionTests
{
    private readonly ISqlConnectionFactory _mockSqlConnectionFactory;
    private readonly IExecuteQueryProvider _mockExecuteQueryProvider;

    public DatabaseCollectionTests()
    {
        _mockSqlConnectionFactory = Substitute.For<ISqlConnectionFactory>();
        _mockExecuteQueryProvider = Substitute.For<IExecuteQueryProvider>();
    }
    
    [Fact]
    public void GetById()
    {
        var databaseCollection = new DatabaseCollection<AuditLog>(
            _mockSqlConnectionFactory, _mockExecuteQueryProvider);

        var getById = databaseCollection.GetById(p => p.Id, Guid.NewGuid());
        var getAll = databaseCollection.GetAll();
    }
}