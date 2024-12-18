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
    
    /// <summary>
    /// Get an entity by its identifier
    /// </summary>
    /// <param name="idPropertySelector">Property selector for id property</param>
    /// <param name="id">id value to match on</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns></returns>
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

    /// <summary>
    /// Get all items
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns></returns>
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

    /// <summary>
    /// Queue creation of the entity
    /// </summary>
    /// <param name="entity">Entity to create</param>
    public void Create(T entity)
    {
        var properties = typeof(T).GetProperties();

        var sqlBuilder = new SqlBuilder<T>()
            .Insert()
            .IntoTable(_tableName);
        foreach (var property in properties)
        {
            sqlBuilder.Value(property.Name, property.GetValue(entity).ToString());
        }

        var databaseQuery = sqlBuilder.BuildQuery();
        
        _executeQueryProvider.AddQuery(databaseQuery);
    }
    
    /// <summary>
    /// Queue replacement of the entity
    /// </summary>
    /// <param name="entity">Entity to replace</param>
    public void Update(T entity)
    {
        const string Sql = "";
        _executeQueryProvider.AddQuery(new DatabaseQuery(Sql, entity));
    }

    /// <summary>
    /// Fetch single item
    /// </summary>
    /// <param name="query">Database query containing custom sql and parameters</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns></returns>
    public async Task<T> QuerySingleRaw(DatabaseQuery query, CancellationToken cancellationToken = default)
    {
        using var connection = await _sqlConnectionFactory.OpenConnection(cancellationToken);
        return await connection.QueryFirstAsync<T>(query.Sql, query.Parameters);
    }
    
    /// <summary>
    /// Fetch collection of items
    /// </summary>
    /// <param name="query">Database query containing custom sql and parameters</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>IEnumerable{T}</returns>
    public async Task<IEnumerable<T>> QueryRaw(DatabaseQuery query, CancellationToken cancellationToken = default)
    {
        using var connection = await _sqlConnectionFactory.OpenConnection(cancellationToken);
        return await connection.QueryAsync<T>(query.Sql, query.Parameters);
    }

    /// <summary>
    /// Queue execution of query
    /// </summary>
    /// <param name="query">Database query containing custom sql and parameters</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public void ExecuteRaw(DatabaseQuery query, CancellationToken cancellationToken = default)
    {
        _executeQueryProvider.AddQuery(query);
    }
}