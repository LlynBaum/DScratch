using DScratch.Client.BrowserInteractions;
using DScratch.Client.Services;
using DScratch.Interactions.CommandHandlers;
using DScratch.Interactions.EventHandlers;

namespace DScratch.Client;

public static class ClientServiceRegistration
{
    public static void RegisterServices(IServiceCollection services, bool isDevelopment)
    { 
        EventHandlerRegistration.Register(services);
        CommandHandlerRegistration.Register(services);
        services.AddScoped<IEditorCommandDispatcher, EditorCommandDispatcher>();
        services.AddScoped<BrowserEventHelper>();
        services.AddScoped<DJsInvoker>();
        services.AddScoped<INodeIdGenerator, NodeIdGenerator>();

        services.AddScoped<IEditorDebugService, EditorDebugService>();
    }
}