namespace DScratch.Client;

public static class ClientServiceRegistration
{
    public static void RegisterServices(IServiceCollection serviceCollection, IConfiguration configuration)
    {
       serviceCollection.AddScoped<INodeIdGenerator, NodeIdGenerator>();
    }
}