using System.Linq.Expressions;

namespace SncSoftware.Extensions.Dapper.SqlBuilders;

public class InsertSqlBuilder<T> : SqlBuilderBase
{
    private string _tableName = string.Empty;
    private readonly Dictionary<string, object> _valuesIndexedByColumnName = new();

    public InsertSqlBuilder<T> IntoTable(string tableName, string schema = "public")
    {
        _tableName = $"{schema}.\"{tableName}\"";
        return this;
    }

    public InsertSqlBuilder<T> Value<TValue>(
        Expression<Func<T, TValue>> propertySelector, string value)
    {
        var propertySelectorBody = propertySelector.Body as MemberExpression;
        var memberName = propertySelectorBody!.Member.Name;
        _valuesIndexedByColumnName.Add(memberName, value);

        return this;
    }

    public InsertSqlBuilder<T> Value(string columnName, string value)
    {
        _valuesIndexedByColumnName.Add(columnName, value);
        return this;
    }

    public override DatabaseQuery BuildQuery()
    {
        var columnsSql = string.Join(',', _valuesIndexedByColumnName.Keys.Select(x => $"\"{x}\""));
        var valuesSql = string.Join(',', _valuesIndexedByColumnName.Keys.Select(x => $"@{x}"));
        var sql = $"INSERT INTO {_tableName} ({columnsSql}) VALUES ({valuesSql})";

        return new DatabaseQuery(sql, _valuesIndexedByColumnName);
    }
}