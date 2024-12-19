using Dapper;

namespace DapperApplication;

public abstract class DatabaseContext
{
    // Ignoring as initialise is called during DI setup
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    private IExecuteQueryProvider _executeQueryProvider;
    private ISqlConnectionFactory _sqlConnectionFactory;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

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
            transaction.Commit();
            _executeQueryProvider.Clear();
        }
        catch (Exception)
        {
            transaction.Rollback();
            throw;
        }
    }
}