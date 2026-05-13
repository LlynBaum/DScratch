namespace DScratch.Client;

public static class ServiceRegistration
{
    public static void RegisterServices(IServiceCollection serviceCollection, IConfiguration configuration, bool serverSide)
    {
       serviceCollection.AddScoped<INodeIdGenerator, NodeIdGenerator>();
    }
}