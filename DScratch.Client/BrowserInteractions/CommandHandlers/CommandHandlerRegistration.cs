namespace DScratch.Client.BrowserInteractions.CommandHandlers;

public static class CommandHandlerRegistration
{
    public static void Register(IServiceCollection services)
    {
        services.AddScoped<IEditorCommandDispatcher, EditorCommandDispatcher>();
    }
}