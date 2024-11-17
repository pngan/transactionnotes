var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.transactionnotes_ApiService>("apiservice");

builder.AddProject<Projects.transactionnotes_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
