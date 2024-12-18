using Dapper;

namespace DapperApplication;

public abstract class DatabaseContext
{
    private IExecuteQueryProvider _executeQueryProvider;
    private ISqlConnectionFactory _sqlConnectionFactory;

    internal void Initialise(
        IExecuteQueryProvider executeQueryProvider,
        ISqlConnectionFactory sqlConnectionFactory)
    {
        _executeQueryProvider = executeQueryProvider;
        _sqlConnectionFactory = sqlConnectionFactory;
    }

    public async Task SaveChanges(CancellationToken cancellationToken)
    {
        var databaseQueries = _executeQueryProvider.GetQueries();
        using var connection = await _sqlConnectionFactory.OpenConnection(cancellationToken);
        using var transaction = connection.BeginTransaction();
        try
        {
            foreach (var databaseQuery in databaseQueries)
            {
                await connection.ExecuteAsync(databaseQuery.Sql, databaseQuery.Parameters, transaction);
            }
        }
        catch (Exception)
        {
            transaction.Rollback();
            throw;
        }
    }
}