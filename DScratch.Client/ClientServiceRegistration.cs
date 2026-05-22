using DScratch.Client.Scripts;
using DScratch.Client.Scripts.EventHandlers;

namespace DScratch.Client;

public static class ClientServiceRegistration
{
    public static void RegisterServices(IServiceCollection services)
    { 
        EventHandlerRegistration.Register(services);
        services.AddScoped<InputEventHelper>();
        services.AddScoped<INodeIdGenerator, NodeIdGenerator>();
    }
}