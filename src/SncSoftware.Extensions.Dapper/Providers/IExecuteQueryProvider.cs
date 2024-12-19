namespace SncSoftware.Extensions.Dapper.Providers;

internal interface IExecuteQueryProvider
{
    void AddQuery(DatabaseQuery query);
    IEnumerable<DatabaseQuery> GetQueries();
    void Clear();
}