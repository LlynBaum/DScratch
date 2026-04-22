using DScratch.Client.JsBridge.ProseMirrorCommand;

namespace DScratch.Client;

public static class ServiceRegistration
{
    public static void RegisterServices(IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.AddTransient<UpdateDispatcher>();
    }
}