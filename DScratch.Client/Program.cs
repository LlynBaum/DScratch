using DScratch.Client;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

ServiceRegistration.RegisterServices(builder.Services, builder.Configuration, false);

await builder.Build().RunAsync();
