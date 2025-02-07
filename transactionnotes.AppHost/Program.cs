using Microsoft.Extensions.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var username = builder.AddParameter("keycloak-username");
var password = builder.AddParameter("keycloak-password", secret: true);
var keycloak = builder.AddKeycloak("keycloak",
        builder.Environment.EnvironmentName == Environments.Development ? 8088 : 80,
                        username,
                        password)
                      .WithDataVolume();

var apiService = builder.AddProject<Projects.transactionnotes_ApiService>("apiservice")
                        .WithReference(keycloak)
                        .WaitFor(keycloak);

builder.AddProject<Projects.transactionnotes_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(keycloak)
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
