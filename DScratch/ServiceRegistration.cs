using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DScratch;

public static class DScratchServiceRegistration
{
    public static void RegisterServices(IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.AddSingleton<DNodeFactory>();
    }
}