using System.Linq.Expressions;
using System.Reflection;
using Dapper;
using DapperApplication.Attributes;
using DapperApplication.Exceptions;
using DapperApplication.Reflection;
using DapperApplication.SqlBuilders;

namespace DapperApplication;

public class DatabaseCollection<T> where T : new()
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;
    private readonly IExecuteQueryProvider _executeQueryProvider;
    private readonly string _tableName;
    
    public DatabaseCollection(
        ISqlConnectionFactory connectionFactory,
        IExecuteQueryProvider executeQueryProvider)
    {
        _sqlConnectionFactory = connectionFactory;
        _executeQueryProvider = executeQueryProvider;

        var tableNameAttribute = typeof(T).GetCustomAttribute<TableNameAttribute>();
        _tableName = tableNameAttribute != null ? tableNameAttribute.TableName : typeof(T).Name.Pluralise();
    }
    
    #region GetById
    
    /// <summary>
    /// Get an entity by its identifier
    /// </summary>
    /// <param name="id">id value to match on</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns></returns>
    public async Task<T> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        var primaryIdentifierProperty = typeof(T).ReflectPrimaryIdentifierProperty();
        if (primaryIdentifierProperty == null)
        {
            throw new MissingPrimaryIdentifierException(typeof(T).Name);
        }
        
        var query = new SqlBuilder<T>()
            .Select()
            .AllProperties()
            .FromTable(_tableName)
            .Where(w => w.PropertyMatches(primaryIdentifierProperty.Name, id.ToString()))
            .BuildQuery();
        
        using var connection = await _sqlConnectionFactory.OpenConnection(cancellationToken);
        return await connection.QueryFirstAsync<T>(query.Sql, query.Parameters);
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
    
    #endregion

    #region GetAll
    
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
    
    #endregion
    
    #region Create

    /// <summary>
    /// Queue creation of the entity
    /// </summary>
    /// <param name="entity">Entity to create</param>
    public void Create(T entity)
    {
        var properties = typeof(T).ReflectProperties();

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
    
    #endregion

    #region Update
    
    /// <summary>
    /// Queue replacement of the entity
    /// </summary>
    /// <param name="entity">Entity to replace</param>
    /// <param name="idPropertySelector"></param>
    public void Update<TValue>(T entity, Expression<Func<T, TValue>> idPropertySelector)
    {
        var properties = typeof(T).ReflectProperties();
        
        var propertySelectorBody = idPropertySelector.Body as MemberExpression;
        var idPropertyName = propertySelectorBody!.Member.Name;
        var idProperty = properties.FirstOrDefault(w => w.Name == idPropertyName);

        var sqlBuilder = new SqlBuilder<T>()
            .Update()
            .Table(_tableName);
        foreach (var property in properties.Except([idProperty]))
        {
            sqlBuilder.SetValue(property.Name, property.GetValue(entity).ToString());
        }
        sqlBuilder.Where(w => w.PropertyMatches(idProperty.Name, idProperty.GetValue(entity).ToString()));
        
        var databaseQuery = sqlBuilder.BuildQuery();
        
        _executeQueryProvider.AddQuery(databaseQuery);
    }
    
    /// <summary>
    /// Queue replacement of the entity
    /// </summary>
    /// <param name="entity">Entity to replace</param>
    public void Update(T entity)
    {
        var properties = typeof(T).ReflectProperties();
        var primaryIdentifierProperty = typeof(T).ReflectPrimaryIdentifierProperty();
        if (primaryIdentifierProperty == null)
        {
            throw new MissingPrimaryIdentifierException(typeof(T).Name);
        }
        
        var sqlBuilder = new SqlBuilder<T>()
            .Update()
            .Table(_tableName);
        foreach (var property in properties.Except([primaryIdentifierProperty]))
        {
            sqlBuilder.SetValue(property.Name, property.GetValue(entity).ToString());
        }
        sqlBuilder.Where(w => w.PropertyMatches(primaryIdentifierProperty.Name, primaryIdentifierProperty.GetValue(entity).ToString()));
        
        var databaseQuery = sqlBuilder.BuildQuery();
        
        _executeQueryProvider.AddQuery(databaseQuery);
    }
    
    #endregion
    
    #region Custom

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
    
    #endregion
}