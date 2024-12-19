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
        Expression<Func<T, TValue>> propertySelector, object value)
    {
        var propertySelectorBody = propertySelector.Body as MemberExpression;
        var memberName = propertySelectorBody!.Member.Name;
        _valuesIndexedByColumnName.Add(memberName, value);

        return this;
    }

    public InsertSqlBuilder<T> Value(string columnName, object value)
    {
        _valuesIndexedByColumnName.Add(columnName, value);
        return this;
    }

    public override DatabaseQuery BuildQuery()
    {
        var columns = new List<string>();
        var valueParameters = new List<string>();
        foreach (var (key, value) in _valuesIndexedByColumnName)
        {
            columns.Add($"\"{key}\"");
            var type = value.GetType();
            if (type == typeof(Guid))
            {
                valueParameters.Add($"@{key}");
                continue;
            }
            valueParameters.Add($"@{key}");
        }

        var columnsSql = string.Join(',', columns);
        var valuesSql = string.Join(',', valueParameters);
        var sql = $"INSERT INTO {_tableName} ({columnsSql}) VALUES ({valuesSql})";

        return new DatabaseQuery(sql, _valuesIndexedByColumnName);
    }
}