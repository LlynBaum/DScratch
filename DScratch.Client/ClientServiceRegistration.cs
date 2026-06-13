using DScratch.Client.BrowserInteractions;
using DScratch.Client.BrowserInteractions.CommandHandlers;
using DScratch.Client.BrowserInteractions.EventHandlers;

namespace DScratch.Client;

public static class ClientServiceRegistration
{
    public static void RegisterServices(IServiceCollection services)
    { 
        EventHandlerRegistration.Register(services);
        CommandHandlerRegistration.Register(services);
        services.AddScoped<InputEventHelper>();
        services.AddScoped<DJsInvoker>();
        services.AddScoped<INodeIdGenerator, NodeIdGenerator>();
    }
}