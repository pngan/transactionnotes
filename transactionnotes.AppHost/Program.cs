using Microsoft.Extensions.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var authority = builder.Configuration["TransNotes:Authority"];
var clientId = builder.Configuration["TransNotes:ClientId"];
var clientSecret = builder.Configuration["TransNotes:ClientSecret"];

var apiService = builder.AddProject<Projects.transactionnotes_ApiService>("apiservice");

builder.AddProject<Projects.transactionnotes_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithEnvironment("TransNotes__Authority", authority)
    .WithEnvironment("TransNotes__ClientId", clientId)
    .WithEnvironment("TransNotes__ClientSecret", clientSecret)
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
