using Datezy.Application;
using Datezy.Infrastructure;
using Datezy.Persistence;
using Datezy.Worker;

var builder = Host.CreateApplicationBuilder(args);

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddPersistence(builder.Configuration);

builder.Services.AddHostedService<Worker>();

var host = builder.Build();

await host.RunAsync();