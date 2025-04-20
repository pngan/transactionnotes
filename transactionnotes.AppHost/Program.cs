using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Protocols.Configuration;

var builder = DistributedApplication.CreateBuilder(args);

var centralDbUsername = builder.Configuration["Database:Central:Username"];
var centralDbPassword = builder.Configuration["Database:Central:Password"];

if (string.IsNullOrEmpty(centralDbUsername) || string.IsNullOrEmpty(centralDbPassword))
{
    throw new InvalidConfigurationException("Please set the Database:Central:Username and Database:Central:Password in the appsettings.json or environment variables.");
}

var databasecentralusername = builder.AddParameter("databasecentralusername", centralDbUsername, publishValueAsDefault:false, secret: true);
var databasecentralpassword = builder.AddParameter("databasecentralpassword", centralDbPassword, publishValueAsDefault: false, secret: true);

var centraldbserver = builder.AddPostgres(
    "centraldbserver", databasecentralusername, databasecentralpassword, 15432)
    .WithPgWeb(pgWeb => pgWeb.WithHostPort(5050))
    .WithPgAdmin(pgAdmin => pgAdmin.WithHostPort(5051));


var centraldb = centraldbserver.AddDatabase("centraldb", "centraldb");

var centralDbService = builder.AddProject<Projects.CentralDb>("centraldbservice")
    .WithReference(centraldb)
    .WaitFor(centraldb);

var apiService = builder.AddProject<Projects.transactionnotes_ApiService>("apiservice")
    .WithReference(centralDbService)
    .WaitForCompletion(centralDbService);

builder.AddProject<Projects.transactionnotes_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
