using System.Linq.Expressions;
using Dapper;
using DapperApplication.SqlBuilders;

namespace DapperApplication;

public class DatabaseCollection<T> where T : new()
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;
    private readonly IExecuteQueryProvider _executeQueryProvider;
    private readonly string _tableName = typeof(T).Name.Pluralise();
    
    public DatabaseCollection(
        ISqlConnectionFactory connectionFactory,
        IExecuteQueryProvider executeQueryProvider)
    {
        _sqlConnectionFactory = connectionFactory;
        _executeQueryProvider = executeQueryProvider;
    }
    
    public async Task<T> GetById<TValue>(Expression<Func<T, TValue>> idPropertySelector, Guid id, CancellationToken cancellationToken = default)
    {
        var query = new SqlBuilder<T>()
            .Select()
            .AllProperties()
            .FromTable(_tableName)
            .Where(w => w.PropertyMatches(idPropertySelector, id.ToString()))
            .BuildQuery();
        
        using var connection = await _sqlConnectionFactory.OpenConnection(cancellationToken);
        return await connection.QueryFirstAsync<T>(query.Sql, query.Parameters);
    }

    public async Task<IEnumerable<T>> GetAll(CancellationToken cancellationToken = default)
    {
        var query = new SqlBuilder<T>()
            .Select()
            .AllProperties()
            .FromTable(_tableName)
            .BuildQuery();
        
        using var connection = await _sqlConnectionFactory.OpenConnection(cancellationToken);
        return await connection.QueryAsync<T>(query.Sql);
    }

    public void Create(T entity)
    {
        const string Sql = "";
        _executeQueryProvider.AddQuery(new DatabaseQuery(Sql, entity));
    }
    
    public void Update(T entity)
    {
        const string Sql = "";
        _executeQueryProvider.AddQuery(new DatabaseQuery(Sql, entity));
    }
}