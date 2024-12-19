namespace SncSoftware.Extensions.Dapper.Providers;

public interface IExecuteQueryProvider
{
    void AddQuery(DatabaseQuery query);
    IEnumerable<DatabaseQuery> GetQueries();
    void Clear();
}