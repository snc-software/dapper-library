using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace DapperApplication;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDatabaseContext<T>(this IServiceCollection services)
    where T: DatabaseContext
    {
        services.AddScoped<ISqlConnectionFactory, PostgresSqlConnectionFactory>();
        services.AddScoped<T>(sp =>
        {
            var connectionFactory = sp.GetRequiredService<ISqlConnectionFactory>();

            var dbContext = (DatabaseContext)Activator.CreateInstance(typeof(T), new DbSettings());

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
                        connectionFactory);
                
                databaseCollection.SetValue(dbContext, databaseCollectionPropertyInstance);
            }

            return (T)dbContext;
        });
        
        
        return services;
    }
}