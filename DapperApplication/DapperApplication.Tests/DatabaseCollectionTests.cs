using NSubstitute;

namespace DapperApplication.Tests;

public class DatabaseCollectionTests
{
    private readonly ISqlConnectionFactory _mockSqlConnectionFactory;
    private readonly IExecuteQueryProvider _mockExecuteQueryProvider;
    
    private readonly DatabaseCollection<AuditLog> _databaseCollection;
    private readonly DatabaseCollection<TypedAuditLog> _typedDatabaseCollection;

    public DatabaseCollectionTests()
    {
        _mockSqlConnectionFactory = Substitute.For<ISqlConnectionFactory>();
        _mockExecuteQueryProvider = Substitute.For<IExecuteQueryProvider>();
        _databaseCollection = new DatabaseCollection<AuditLog>(
            _mockSqlConnectionFactory, _mockExecuteQueryProvider);
        _typedDatabaseCollection = new DatabaseCollection<TypedAuditLog>(
            _mockSqlConnectionFactory, _mockExecuteQueryProvider);
    }
    
    [Fact]
    public void GetById()
    {
        var getById = _databaseCollection.GetById(p => p.Id, Guid.NewGuid());
        var getByIdTyped = _typedDatabaseCollection.GetById(Guid.NewGuid());
    }
    
    [Fact]
    public void GetAll()
    {
        var getAll = _databaseCollection.GetAll();
    }
    
    [Fact]
    public void Create()
    {
        _databaseCollection.Create(new AuditLog
        {
            Id = Guid.NewGuid(),
            Body = "Hello i am the body"
        });
    }
    
    [Fact]
    public void Update()
    {
        _databaseCollection.Update(new AuditLog
        {
            Id = Guid.NewGuid(),
            Body = "Hello i am the body"
        }, x => x.Id);
        
        _typedDatabaseCollection.Update(new TypedAuditLog
        {
            Id = Guid.NewGuid(),
            Body = "Hello i am the body"
        });
    }
}