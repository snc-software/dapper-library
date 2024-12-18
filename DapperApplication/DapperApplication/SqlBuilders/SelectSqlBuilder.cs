using System.Linq.Expressions;

namespace DapperApplication.SqlBuilders;

public class SelectSqlBuilder<T> : SqlBuilderBase
{
    private readonly List<string> _columns = [];
    private string _fromTable = string.Empty;
    private readonly WhereClauseSqlBuilder<T> _whereClauseBuilder = new();
    private const string BaseSqlFormat = "SELECT {0} FROM {1}";

    public SelectSqlBuilder<T> AllProperties()
    {
        _columns.Add("*");
        return this;
    }
    
    public SelectSqlBuilder<T> Property<TValue>(Expression<Func<T, TValue>> propertySelector)
    {
        var propertySelectorBody = propertySelector.Body as MemberExpression;
        var memberName = propertySelectorBody!.Member.Name;
        _columns.Add($"\"{memberName}\"");
        
        return this;
    }
    
    public SelectSqlBuilder<T> Property(string columnName)
    {
        _columns.Add($"\"{columnName}\"");
        return this;
    }

    public SelectSqlBuilder<T> FromTable(string tableName, string schema = "public")
    {
        _fromTable = $"{schema}.\"{tableName}\"";
        return this;
    }

    public SelectSqlBuilder<T> Where(Action<WhereClauseSqlBuilder<T>> whereClauseBuilder)
    {
        whereClauseBuilder(_whereClauseBuilder);
        return this;
    }

    public override DatabaseQuery BuildQuery()
    {
        var sql = string.Format(BaseSqlFormat, string.Join(',', _columns), _fromTable);
        
        var whereClauseBuilder = _whereClauseBuilder.Build();
        if (!string.IsNullOrWhiteSpace(whereClauseBuilder.WhereClause))
        {
            sql += $" WHERE {whereClauseBuilder.WhereClause}";
        }
        return new DatabaseQuery(sql, whereClauseBuilder.Parameters.Count != 0 ? whereClauseBuilder.Parameters : null);
    }
}