using System.Data;
using Npgsql;

namespace DapperApplication;

public class PostgresSqlConnectionFactory : ISqlConnectionFactory
{
    private readonly NpgsqlDataSource _npgSqlDataSource;

    public PostgresSqlConnectionFactory(DbSettings dbSettings)
    {
        var dataSourceBuilder = new NpgsqlDataSourceBuilder(dbSettings.ConnectionString);
        _npgSqlDataSource = dataSourceBuilder.Build();
    }
    
    public async Task<IDbConnection> OpenConnection(CancellationToken cancellationToken)
    {
        return await _npgSqlDataSource.OpenConnectionAsync(cancellationToken);
    }
}