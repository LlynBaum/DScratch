using DScratch.Client.JsBridge;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DScratch.Client;

public static class ServiceRegistration
{
    public static void RegisterServices(IServiceCollection serviceCollection, IConfiguration configuration, bool serverSide)
    {
        serviceCollection.AddTransient<IPmBridge, PmBridge>();
        
        if (serverSide)
        {
            // We can not use PostMirror during pre-render. So we replace the service with a fake.
            serviceCollection.Replace(new ServiceDescriptor(typeof(IPmBridge), typeof(PmBridgeServer), ServiceLifetime.Transient));
        }
    }
}