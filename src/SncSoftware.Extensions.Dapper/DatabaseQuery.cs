namespace SncSoftware.Extensions.Dapper;

public record DatabaseQuery
{
    public DatabaseQuery(string sql, object? parameters)
    {
        Sql = sql;
        Parameters = parameters;
    }

    public DatabaseQuery()
    {
        Sql = string.Empty;
        Parameters = null;
    }

    public string Sql { get; set; }
    public object? Parameters { get; set; }
}