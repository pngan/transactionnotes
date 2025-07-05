using Aspire.Hosting;
using Microsoft.IdentityModel.Protocols.Configuration;

var builder = DistributedApplication.CreateBuilder(args);

var centralDbUsername = builder.Configuration["Database:Central:Admin:Username"];
var centralDbPassword = builder.Configuration["Database:Central:Admin:Password"];
var centralDatabaseName = builder.Configuration["Database:Central:Name"];

if (string.IsNullOrEmpty(centralDbUsername) || string.IsNullOrEmpty(centralDbPassword))
{
    throw new InvalidConfigurationException("Please set the Database:Central:Admin:Username and Database:Central:Admin:Password in the appsettings.json or environment variables.");
}

var databasecentralusername = builder.AddParameter("databasecentralusername", centralDbUsername, publishValueAsDefault:false, secret: true);
var databasecentralpassword = builder.AddParameter("databasecentralpassword", centralDbPassword, publishValueAsDefault: false, secret: true);

var centraldbserver = builder.AddPostgres(
    "centraldbserver", databasecentralusername, databasecentralpassword, 15432)
    .WithPgAdmin(pgAdmin => pgAdmin.WithHostPort(5051));


var centraldb = centraldbserver.AddDatabase(centralDatabaseName);

var centralDbMigration = builder.AddProject<Projects.centraldb_migration>("centraldb-migration")
    .WithReference(centraldb)
    .WaitFor(centraldb);

var apiService = builder.AddProject<Projects.transactionnotes_ApiService>("apiservice")
    .WithReference(centralDbMigration)
    .WaitForCompletion(centralDbMigration);

var centralApi = builder.AddProject<Projects.central_api>("centralapi")
    .WithReference(centralDbMigration)
    .WaitForCompletion(centralDbMigration);

builder.AddProject<Projects.transactionnotes_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService)
    .WaitFor(apiService)
    .WithReference(centralApi)
    .WaitFor(centralApi);

//builder.AddProject<Projects.transactionnotes_Tests>("tests");
builder.Build().Run();
