using DbUp;
using Npgsql;

namespace SncSoftware.Extensions.Dapper.ServiceTests.Infrastructure.Persistence;

public class PostgresDatabaseMigrator
{
    private readonly string _connectionString;
    private readonly string _databaseName;

    public PostgresDatabaseMigrator(string connectionString, string databaseName)
    {
        _connectionString = connectionString;
        _databaseName = databaseName;
    }

    public void EnsureCreated()
    {
        var builder = new NpgsqlConnectionStringBuilder(_connectionString)
        {
            Database = _databaseName
        };

        EnsureDatabase.For.PostgresqlDatabase(builder.ToString());
        var upgrader = DeployChanges.To.PostgresqlDatabase(builder.ToString())
            .WithScriptsEmbeddedInAssembly(GetType().Assembly)
            .LogToConsole()
            .Build();

        if (upgrader.IsUpgradeRequired())
        {
            var upgradeResult = upgrader.PerformUpgrade();
            if (!upgradeResult.Successful)
            {
                Console.WriteLine(upgradeResult.Error);
            }
        }
    }

    public void EnsureDeleted()
    {
        var builder = new NpgsqlConnectionStringBuilder(_connectionString)
        {
            Database = "postgres"
        };

        using var connection = new NpgsqlConnection(builder.ToString());
        connection.Open();

        using var terminateConnectionCommand = new NpgsqlCommand(
            $"SELECT pg_terminate_backend(pid) FROM pg_stat_activity WHERE datname ='{_databaseName}'", connection);
        terminateConnectionCommand.ExecuteNonQuery();

        using var dropDatabaseCommand = new NpgsqlCommand($"DROP DATABASE \"{_databaseName}\"", connection);
        dropDatabaseCommand.ExecuteNonQuery();

        connection.Close();
    }
}