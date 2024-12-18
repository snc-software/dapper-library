namespace DapperApplication.SqlBuilders;

public class SqlBuilder<T>
{
    public SelectSqlBuilder<T> Select()
    {
        return new SelectSqlBuilder<T>();
    }

    public InsertSqlBuilder<T> Insert()
    {
        return new InsertSqlBuilder<T>();
    }
}