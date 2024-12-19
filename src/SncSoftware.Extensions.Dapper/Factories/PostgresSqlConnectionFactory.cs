using System.Data;
using Npgsql;

namespace SncSoftware.Extensions.Dapper.Factories;

public class PostgresSqlConnectionFactory : ISqlConnectionFactory
{
    private readonly NpgsqlDataSource _npgSqlDataSource;

    public PostgresSqlConnectionFactory(DatabaseSettings databaseSettings)
    {
        var dataSourceBuilder = new NpgsqlDataSourceBuilder(databaseSettings.ConnectionString);
        _npgSqlDataSource = dataSourceBuilder.Build();
    }

    public async Task<IDbConnection> OpenConnection(CancellationToken cancellationToken)
    {
        return await _npgSqlDataSource.OpenConnectionAsync(cancellationToken);
    }
}