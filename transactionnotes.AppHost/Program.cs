var builder = DistributedApplication.CreateBuilder(args);

var username = builder.AddParameter("keycloak-username");
var password = builder.AddParameter("keycloak-password", secret: true);
var keycloak = builder.AddKeycloak("keycloak", 8088,
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
