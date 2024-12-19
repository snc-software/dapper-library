using System.Linq.Expressions;

namespace DapperApplication.SqlBuilders;

public class UpdateSqlBuilder<T> : SqlBuilderBase
{
    private readonly WhereClauseSqlBuilder<T> _whereClauseBuilder = new();
    private string _tableName = string.Empty;
    private readonly Dictionary<string, object> _valuesIndexedByColumnName = new();

    public UpdateSqlBuilder<T> Table(string tableName, string schema = "public")
    {
        _tableName = $"{schema}.\"{tableName}\"";
        return this;
    }
    
    public UpdateSqlBuilder<T> SetValue<TValue>(Expression<Func<T, TValue>> propertySelector, string value)
    {
        var propertySelectorBody = propertySelector.Body as MemberExpression;
        var memberName = propertySelectorBody!.Member.Name;
        _valuesIndexedByColumnName.Add(memberName, value);
        
        return this;
    }
    
    public UpdateSqlBuilder<T> SetValue(string columnName, string value)
    {
        _valuesIndexedByColumnName.Add(columnName, value);
        return this;
    }
    
    public UpdateSqlBuilder<T> Where(Action<WhereClauseSqlBuilder<T>> whereClauseBuilder)
    {
        whereClauseBuilder(_whereClauseBuilder);
        return this;
    }
    
    public override DatabaseQuery BuildQuery()
    {
        var sql = $"UPDATE {_tableName}";
        var propertySql = string.Join(", ", _valuesIndexedByColumnName.Keys.Select(p => $"\"{p}\"=@{p}"));
        sql += $" SET {propertySql}";
        var parameters = _valuesIndexedByColumnName;
        
        var whereClauseBuilder = _whereClauseBuilder.Build();
        
        if (!string.IsNullOrWhiteSpace(whereClauseBuilder.WhereClause))
        {
            sql += $" {whereClauseBuilder.WhereClause}";
            var allParameters = new List<Dictionary<string, object>>()
            {
                _valuesIndexedByColumnName, whereClauseBuilder.Parameters
            };
            parameters = allParameters.SelectMany(dict => dict).ToDictionary();
        }

        return new DatabaseQuery(sql, parameters);
    }
}