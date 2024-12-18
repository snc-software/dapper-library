namespace DapperApplication;

public interface IExecuteQueryProvider
{
    void AddQuery(DatabaseQuery query);
    IEnumerable<DatabaseQuery> GetQueries();
}