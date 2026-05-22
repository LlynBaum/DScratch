using DScratch;
using DScratch.Client;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

DScratchServiceRegistration.RegisterServices(builder.Services);
ClientServiceRegistration.RegisterServices(builder.Services, builder.Configuration);

await builder.Build().RunAsync();
