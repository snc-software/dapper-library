using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using SncSoftware.Extensions.Dapper.Factories;
using SncSoftware.Extensions.Dapper.Providers;

namespace SncSoftware.Extensions.Dapper;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDatabaseContext<T>(
        this IServiceCollection services)
        where T : DatabaseContext
    {
        var dbSettings = new DatabaseSettings();
        services.AddSingleton(dbSettings);
        services.AddScoped<ISqlConnectionFactory, PostgresSqlConnectionFactory>();
        services.AddScoped<IExecuteQueryProvider, ExecuteQueryProvider>();
        services.AddScoped<T>(sp =>
        {
            var connectionFactory = sp.GetRequiredService<ISqlConnectionFactory>();
            var executeQueryProvider = sp.GetRequiredService<IExecuteQueryProvider>();

            var dbContext = (DatabaseContext)Activator.CreateInstance(typeof(T))!;
            dbContext.Initialise(executeQueryProvider, connectionFactory);

            var databaseCollectionProperties = typeof(T)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.PropertyType.IsGenericType &&
                            p.PropertyType.GetGenericTypeDefinition() == typeof(DatabaseCollection<>));

            foreach (var databaseCollection in databaseCollectionProperties)
            {
                var genericTypeArgument = databaseCollection.PropertyType.GetGenericArguments()[0];

                var databaseCollectionPropertyInstance =
                    Activator.CreateInstance(
                        typeof(DatabaseCollection<>).MakeGenericType(genericTypeArgument),
                        connectionFactory, executeQueryProvider);

                databaseCollection.SetValue(dbContext, databaseCollectionPropertyInstance);
            }

            return (T)dbContext;
        });


        return services;
    }
}