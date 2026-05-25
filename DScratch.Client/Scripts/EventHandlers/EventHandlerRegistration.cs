namespace DScratch.Client.Scripts.EventHandlers;

public static class EventHandlerRegistration
{
    public static void Register(IServiceCollection services)
    {
        services.AddKeyedScoped<IEditorEventHandler, InsertTextHandler>(InsertTextHandler.EventName);
        services.AddKeyedScoped<IEditorEventHandler, DeleteContentBackwardHandler>(DeleteContentBackwardHandler.EventName);
        services.AddKeyedScoped<IEditorEventHandler, DeleteContentForwardHandler>(DeleteContentForwardHandler.EventName);
    }
}