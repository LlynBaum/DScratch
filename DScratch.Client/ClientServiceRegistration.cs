using DScratch.Client.Scripts.EventHandlers;

namespace DScratch.Client;

public static class ClientServiceRegistration
{
    public static void RegisterServices(IServiceCollection services, IConfiguration configuration)
    { 
        EventHandlerRegistration.Register(services); 
        services.AddScoped<INodeIdGenerator, NodeIdGenerator>();
    }
}