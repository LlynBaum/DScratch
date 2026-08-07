using DScratch.Interactions.CommandHandlers.Commands;
using DScratch.Interactions.CommandHandlers.Handlers;
using DScratch.Marks;
using Microsoft.Extensions.DependencyInjection;

namespace DScratch.Interactions.CommandHandlers;

public static class CommandHandlerRegistration
{
    public static void Register(IServiceCollection services)
    {
        services.AddScoped<IEditorCommandHandler<ChangeBlockTypeCommand>, ChangeBlockTypeHandler>();
        services.AddScoped<IEditorCommandHandler<IMarkCommand>, UpdateMarkHandler>();
        services.AddScoped<IEditorCommandHandler<AddLinkCommand>, AddLinkHandler>();
        services.AddScoped<IEditorCommandHandler<RemoveLinkCommand>, RemoveLinkHandler>();
    }
}