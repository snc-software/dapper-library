using System.Linq.Expressions;
using System.Text;

namespace SncSoftware.Extensions.Dapper.SqlBuilders;

public class WhereClauseSqlBuilder<T>
{
    private readonly StringBuilder _whereClauseStringBuilder = new();
    private readonly Dictionary<string, object> _parameters = new();

    public WhereClauseSqlBuilder<T> And()
    {
        _whereClauseStringBuilder.Append(" AND ");
        return this;
    }

    public WhereClauseSqlBuilder<T> PropertyMatches<TValue>(Expression<Func<T, TValue>> propertySelector, string value)
    {
        var propertySelectorBody = propertySelector.Body as MemberExpression;
        var memberName = propertySelectorBody!.Member.Name;
        _whereClauseStringBuilder.Append($"\"{memberName}\" = @{memberName}");
        _parameters.Add(memberName, value);
        return this;
    }

    public WhereClauseSqlBuilder<T> PropertyMatches<TValue>(Expression<Func<T, TValue>> propertySelector, Guid value)
    {
        var propertySelectorBody = propertySelector.Body as MemberExpression;
        var memberName = propertySelectorBody!.Member.Name;
        _whereClauseStringBuilder.Append($"\"{memberName}\" = @{memberName}::uuid");
        _parameters.Add(memberName, value);
        return this;
    }

    public WhereClauseSqlBuilder<T> PropertyMatches(string propertyName, string value)
    {
        _whereClauseStringBuilder.Append($"\"{propertyName}\" = @{propertyName}");
        _parameters.Add(propertyName, value);
        return this;
    }

    public WhereClauseSqlBuilder<T> PropertyMatches(string propertyName, Guid value)
    {
        _whereClauseStringBuilder.Append($"\"{propertyName}\" = @{propertyName}::uuid");
        _parameters.Add(propertyName, value);
        return this;
    }

    public (string WhereClause, Dictionary<string, object> Parameters) Build()
    {
        var whereClause = $"WHERE {_whereClauseStringBuilder}";
        return (whereClause, _parameters);
    }
}