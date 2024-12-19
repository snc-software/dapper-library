namespace SncSoftware.Extensions.Dapper.SqlBuilders;

public class DeleteSqlBuilder<T> : SqlBuilderBase
{
    private string _tableName = string.Empty;
    private readonly WhereClauseSqlBuilder<T> _whereClauseBuilder = new();

    public DeleteSqlBuilder<T> FromTable(string tableName, string schema = "public")
    {
        _tableName = $"{schema}.\"{tableName}\"";
        return this;
    }

    public DeleteSqlBuilder<T> Where(Action<WhereClauseSqlBuilder<T>> whereClauseBuilder)
    {
        whereClauseBuilder(_whereClauseBuilder);
        return this;
    }

    public override DatabaseQuery BuildQuery()
    {
        var sql = $"DELETE FROM {_tableName}";

        var whereClauseBuilder = _whereClauseBuilder.Build();
        if (!string.IsNullOrWhiteSpace(whereClauseBuilder.WhereClause))
        {
            sql += $" WHERE {whereClauseBuilder.WhereClause}";
        }
        return new DatabaseQuery(sql, whereClauseBuilder.Parameters.Count != 0 ? whereClauseBuilder.Parameters : null);
    }
}