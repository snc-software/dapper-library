namespace SncSoftware.Extensions.Dapper.Providers;

internal class ExecuteQueryProvider : IExecuteQueryProvider
{
    private readonly List<DatabaseQuery> _queries = [];

    public void AddQuery(DatabaseQuery query)
    {
        _queries.Add(query);
    }

    public IEnumerable<DatabaseQuery> GetQueries()
    {
        return _queries;
    }

    public void Clear()
    {
        _queries.Clear();
    }
}