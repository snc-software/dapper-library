using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using SncSoftware.Extensions.Dapper.ServiceTests.Contracts;

namespace SncSoftware.Extensions.Dapper.ServiceTests.Infrastructure.Persistence;

public class EntityWithAttributesPostgresProvider
{
    public static async Task<EntityWithAttributes?> Get(Guid id, string? database = null)
    {
        const string Sql = """
                           SELECT* FROM public."AttributeEntities" WHERE "Id" = @id LIMIT 1 
                           """;
        var postgresSettings = Settings.DatabaseSettings;
        var postgresDatabase = database ?? Settings.DatabaseName;

        var connectionStringBuilder = new NpgsqlConnectionStringBuilder(postgresSettings.ConnectionString)
        {
            Database = postgresDatabase
        };
        var dataSource = new NpgsqlDataSourceBuilder(connectionStringBuilder.ConnectionString).Build();

        await using var connection = await dataSource.OpenConnectionAsync();
        return await connection.QuerySingleOrDefaultAsync<EntityWithAttributes>(Sql, new
        {
            id
        });
    }

    public static async Task<IEnumerable<EntityWithAttributes>> GetAll(string? database = null)
    {
        const string Sql = """
                           SELECT* FROM public."AttributeEntities"
                           """;
        var postgresSettings = Settings.DatabaseSettings;
        var postgresDatabase = database ?? Settings.DatabaseName;

        var connectionStringBuilder = new NpgsqlConnectionStringBuilder(postgresSettings.ConnectionString)
        {
            Database = postgresDatabase
        };
        var dataSource = new NpgsqlDataSourceBuilder(connectionStringBuilder.ConnectionString).Build();

        await using var connection = await dataSource.OpenConnectionAsync();
        return await connection.QueryAsync<EntityWithAttributes>(Sql);
    }

    public static async Task Insert(EntityWithAttributes entityWithAttributes, string? database = null)
    {
        const string Sql = """
                           INSERT INTO public."AttributeEntities"("Id", "Description", "Age", "Enabled")
                           VALUES(@Id, @Description, @Age, @Enabled)
                           """;
        var postgresSettings = Settings.DatabaseSettings;
        var postgresDatabase = database ?? Settings.DatabaseName;

        var connectionStringBuilder = new NpgsqlConnectionStringBuilder(postgresSettings.ConnectionString)
        {
            Database = postgresDatabase
        };
        var dataSource = new NpgsqlDataSourceBuilder(connectionStringBuilder.ConnectionString).Build();

        await using var connection = await dataSource.OpenConnectionAsync();
        await connection.ExecuteAsync(Sql, entityWithAttributes);
    }
}