using Microsoft.Extensions.DependencyInjection;
using SncSoftware.Extensions.Dapper.ServiceTests.Contracts;

namespace SncSoftware.Extensions.Dapper.ServiceTests.Infrastructure;

public class ServiceCollectionFactory
{
    private static ServiceCollectionFactory s_instance;

    public ServiceCollectionFactory(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
    }

    public IServiceProvider ServiceProvider { get; private set; }

    public static ServiceCollectionFactory Instance
    {
        get
        {
            if (s_instance is not null)
            {
                return s_instance;
            }

            throw new Exception("Instance is not initialized");
        }
    }

    public static void Initialize()
    {
        var services = new ServiceCollection();
        services.AddDatabaseContext<TestDatabaseContext>(Settings.Configuration);
        var serviceProvider = services.BuildServiceProvider();
        s_instance = new ServiceCollectionFactory(serviceProvider);
    }
}