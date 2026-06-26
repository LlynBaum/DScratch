using DScratch.Interactions.EventHandlers.Events;
using Microsoft.Extensions.DependencyInjection;

namespace DScratch.Interactions.EventHandlers;

public static class EventHandlerRegistration
{
    public static void Register(IServiceCollection services)
    {
        services.AddKeyedScoped<IEditorEventHandler, InsertTextHandler>(InsertTextHandler.EventName);
        services.AddKeyedScoped<IEditorEventHandler, InsertParagraphHandler>(InsertParagraphHandler.EventName);
        services.AddKeyedScoped<IEditorEventHandler, DeleteContentBackwardHandler>(DeleteContentBackwardHandler.EventName);
        services.AddKeyedScoped<IEditorEventHandler, DeleteContentForwardHandler>(DeleteContentForwardHandler.EventName);
        services.AddKeyedScoped<IEditorEventHandler, DeleteWordBackwardHandler>(DeleteWordBackwardHandler.EventName);
        services.AddKeyedScoped<IEditorEventHandler, DeleteWordForwardHandler>(DeleteWordForwardHandler.EventName);
    }
}