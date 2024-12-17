using System.Reflection;

namespace DapperApplication;

public abstract class DatabaseContext
{
    protected DatabaseContext(DbSettings settings)
    {
        // get all db collection properties
        // initialise
    }

    public Task SaveChanges(CancellationToken cancellationToken)
    {
        var properties = GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.PropertyType.IsGenericType &&
                        p.PropertyType.GetGenericTypeDefinition() == typeof(DatabaseCollection<>));
        // Get all database collections
        foreach (var property in properties)
        {
            // get all outstanding queries
        }
        
        // build all sql commands
        // open transaction 
        // commit

        return Task.CompletedTask;
    }
}