using System.Linq.Expressions;
using System.Reflection;
using Dapper;
using SncSoftware.Extensions.Dapper.Reflection;
using SncSoftware.Extensions.Dapper.Attributes;
using SncSoftware.Extensions.Dapper.Exceptions;
using SncSoftware.Extensions.Dapper.Extensions;
using SncSoftware.Extensions.Dapper.Factories;
using SncSoftware.Extensions.Dapper.Providers;
using SncSoftware.Extensions.Dapper.SqlBuilders;

namespace SncSoftware.Extensions.Dapper;

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
    public async Task<T?> GetById(Guid id, CancellationToken cancellationToken = default)
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
            .Where(w => w.PropertyMatches(primaryIdentifierProperty.Name, id))
            .BuildQuery();

        using var connection = await _sqlConnectionFactory.OpenConnection(cancellationToken);
        return await connection.QuerySingleOrDefaultAsync<T>(query.Sql, query.Parameters);
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
            .Where(w => w.PropertyMatches(idPropertySelector, id))
            .BuildQuery();

        using var connection = await _sqlConnectionFactory.OpenConnection(cancellationToken);
        return await connection.QueryFirstAsync<T>(query.Sql, query.Parameters);
    }

    #endregion

    #region Get Multiple

    // TODO - Implement Order By
    /// <summary>
    /// Get all items
    /// </summary>
    /// <param name="orderByPropertySelector"></param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns></returns>
    public async Task<IEnumerable<T>> GetAll(
        CancellationToken cancellationToken = default)
    {
        var query = new SqlBuilder<T>()
            .Select()
            .AllProperties()
            .FromTable(_tableName)
            .BuildQuery();

        using var connection = await _sqlConnectionFactory.OpenConnection(cancellationToken);
        return await connection.QueryAsync<T>(query.Sql);
    }
    
    //TODO - IMPLEMENT
    
    /// <summary>
    /// Get a page of items
    /// </summary>
    /// <param name="orderByPropertySelector"></param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns></returns>
    public async Task<IEnumerable<T>> GetPage<TValue>(
        int page,
        int pageSize,
        Expression<Func<T, TValue>>? orderByPropertySelector = null,
        CancellationToken cancellationToken = default)
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
    /// Get a page of items
    /// </summary>
    /// <param name="orderByPropertySelector"></param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns></returns>
    public async Task<IEnumerable<T>> GetFilteredPage<TValue>(
        int page,
        int pageSize,
        WhereClauseSqlBuilder<T>? filter = null,
        Expression<Func<T, TValue>>? orderByPropertySelector = null,
        CancellationToken cancellationToken = default)
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
            sqlBuilder.Value(property.Name, property.GetValue(entity));
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
            sqlBuilder.SetValue(property.Name, property.GetValue(entity));
        }
        sqlBuilder.Where(w => w.PropertyMatches(idProperty.Name, idProperty.GetValue(entity)));

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
            sqlBuilder.SetValue(property.Name, property.GetValue(entity));
        }
        sqlBuilder.Where(w => w.PropertyMatches(primaryIdentifierProperty.Name, primaryIdentifierProperty.GetValue(entity)));

        var databaseQuery = sqlBuilder.BuildQuery();

        _executeQueryProvider.AddQuery(databaseQuery);
    }

    #endregion

    #region Delete

    /// <summary>
    /// Delete an entity by its identifier
    /// </summary>
    /// <param name="id">id value to match on</param>
    /// <returns></returns>
    public void DeleteById(Guid id)
    {
        var primaryIdentifierProperty = typeof(T).ReflectPrimaryIdentifierProperty();
        if (primaryIdentifierProperty == null)
        {
            throw new MissingPrimaryIdentifierException(typeof(T).Name);
        }
        var query = new SqlBuilder<T>()
            .Delete()
            .FromTable(_tableName)
            .Where(w => w.PropertyMatches(primaryIdentifierProperty.Name, id))
            .BuildQuery();

        _executeQueryProvider.AddQuery(query);
    }

    /// <summary>
    /// Delete an entity by its identifier
    /// </summary>
    /// <param name="idPropertySelector">Property selector for id property</param>
    /// <param name="id">id value to match on</param>
    /// <returns></returns>
    public void DeleteById<TValue>(Expression<Func<T, TValue>> idPropertySelector, Guid id)
    {
        var query = new SqlBuilder<T>()
            .Delete()
            .FromTable(_tableName)
            .Where(w => w.PropertyMatches(idPropertySelector, id))
            .BuildQuery();

        _executeQueryProvider.AddQuery(query);
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