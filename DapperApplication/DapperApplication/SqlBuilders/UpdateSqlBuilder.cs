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
    
    public UpdateSqlBuilder<T> Property(string columnName, string value)
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
        // UPDATE public."Items"
        // SET "Id"=?, "Hash"=?, "Title"=?, "DateTime"=?, "Category"=?, "Size"=?, "Imdb"=?
        // WHERE <condition>;
        
        var sql = $"UPDATE {_tableName}";
        var propertySql = string.Join(", ", _valuesIndexedByColumnName.Keys.Select(p => $"\"{p}\"=@{p}"));
        sql += $" SET {propertySql}";
        var whereClauseBuilder = _whereClauseBuilder.Build();
        if (!string.IsNullOrWhiteSpace(whereClauseBuilder.WhereClause))
        {
            sql += $" {whereClauseBuilder.WhereClause}";
        }

        return new DatabaseQuery(sql, _valuesIndexedByColumnName);
    }
}