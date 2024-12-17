using System.Data;
using Npgsql;

namespace DapperApplication;

public class PostgresSqlConnectionFactory : ISqlConnectionFactory
{
    public IDbConnection GetConnection()
    {
        return new NpgsqlConnection();
    }
}