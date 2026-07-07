using DScratch.Client.BrowserInteractions;
using DScratch.Client.BrowserInteractions.Rendering;
using DScratch.Client.Services;
using DScratch.Interactions.CommandHandlers;
using DScratch.Interactions.EventHandlers;
using DScratch.LayoutEngine;

namespace DScratch.Client;

public static class ClientServiceRegistration
{
    public static void RegisterServices(IServiceCollection services)
    { 
        EventHandlerRegistration.Register(services);
        services.AddScoped<IEditorCommandDispatcher, EditorCommandDispatcher>();
        services.AddScoped<InputEventHelper>();
        services.AddScoped<DJsInvoker>();
        services.AddScoped<INodeIdGenerator, NodeIdGenerator>();
        services.AddScoped<ILayoutRenderer, LayoutRenderer>();
        
#if DEBUG // TODO replace by checking env. Do that everywhere
        services.AddScoped<EditorDebugService>();
#endif
    }
}