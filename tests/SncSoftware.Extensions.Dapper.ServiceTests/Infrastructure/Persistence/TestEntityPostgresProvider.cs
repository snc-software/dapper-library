using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using SncSoftware.Extensions.Dapper.ServiceTests.Contracts;

namespace SncSoftware.Extensions.Dapper.ServiceTests.Infrastructure.Persistence;

public class TestEntityPostgresProvider
{
    public static async Task Insert(TestEntity entity)
    {
        const string Sql = """
                           INSERT INTO public."TestEntities" ("Id", "Description", "Age", "Enabled")
                           VALUES(@Id, @Description, @Age, @Enabled)
                           """;
        var postgresSettings = Settings.Configuration.GetSection("Postgres").Get<DatabaseSettings>();
    var postgresDatabase = Settings.DatabaseName;

    var connectionStringBuilder = new NpgsqlConnectionStringBuilder(postgresSettings.ConnectionString)
    {
        Database = postgresDatabase
    };
    var dataSource = new NpgsqlDataSourceBuilder(connectionStringBuilder.ConnectionString).Build();

    await using var connection = await dataSource.OpenConnectionAsync();
await connection.ExecuteAsync(Sql, entity);
}
}