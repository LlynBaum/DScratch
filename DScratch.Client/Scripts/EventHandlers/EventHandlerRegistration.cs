namespace DScratch.Client.Scripts.EventHandlers;

public static class EventHandlerRegistration
{
    public static void Register(IServiceCollection services)
    {
        services.AddKeyedSingleton<IEditorEventHandler, InsertTextHandler>(InsertTextHandler.EventName);
        services.AddKeyedSingleton<IEditorEventHandler, DeleteContentBackwardHandler>(DeleteContentBackwardHandler.EventName);
    }
}