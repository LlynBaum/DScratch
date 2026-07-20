using DScratch.Interactions.CommandHandlers.Commands;
using DScratch.Interactions.CommandHandlers.Handlers;
using Microsoft.Extensions.DependencyInjection;

namespace DScratch.Interactions.CommandHandlers;

public class CommandHandlerRegistration
{
    public static void Register(IServiceCollection services)
    {
        services.AddScoped<IEditorCommandHandler<ChangeBlockTypeCommand>, ChangeBlockTypeHandler>();
        services.AddScoped<IEditorCommandHandler<UpdateMarkCommand>, UpdateMarkHandler>();
    }
}