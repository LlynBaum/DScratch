using DScratch.Client;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddSingleton<Wasm>();

var app = builder.Build();

await app.RunAsync();
