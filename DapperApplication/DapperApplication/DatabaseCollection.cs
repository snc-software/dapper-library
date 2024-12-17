using Dapper;

namespace DapperApplication;

public class DatabaseCollection<T> where T : new()
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;
    public DatabaseCollection(ISqlConnectionFactory connectionFactory)
    {
        Queries = [];
        _sqlConnectionFactory = connectionFactory;
    }
    
    protected IEnumerable<DatabaseQuery> Queries {get; set;}
    
    
    public async Task<T> GetById(Guid id)
    {
        using var connection = _sqlConnectionFactory.GetConnection();
        return await connection.QueryFirstAsync<T>("SELECT * FROM \"AuditLogs\" where \"Id\" = @id",
            new{ id });
    }

    public async Task<IEnumerable<T>> GetAll()
    {
        using var connection = _sqlConnectionFactory.GetConnection();
        return await connection.QueryAsync<T>("SELECT * FROM \"AuditLogs\"");
    }
}