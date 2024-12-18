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

    public UpdateSqlBuilder<T> Update()
    {
        return new UpdateSqlBuilder<T>();
    }

    public DeleteSqlBuilder<T> Delete()
    {
        return new DeleteSqlBuilder<T>();
    }
}