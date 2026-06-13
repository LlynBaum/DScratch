using DScratch.Client.BrowserInteractions.EventHandlers.Events;

namespace DScratch.Client.BrowserInteractions.EventHandlers;

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