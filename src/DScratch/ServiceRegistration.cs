using DScratch.Interactions.UserStates;
using DScratch.Rendering;
using Microsoft.Extensions.DependencyInjection;

namespace DScratch;

public static class DScratchServiceRegistration
{
    public static void RegisterServices(IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<INodeFactory, DNodeFactory>();
        serviceCollection.AddScoped<IDScratchService, DScratchService>();
        serviceCollection.AddScoped<IUserStateService, UserStateService>();
    }
}