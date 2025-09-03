#pragma warning disable ASPIRECOSMOSDB001 

var builder = DistributedApplication.CreateBuilder(args);

builder.AddAzureContainerAppEnvironment("aca-env");

var storage = builder.AddAzureStorage("storage");
var blob = storage.AddBlobContainer("images");

var cosmosDb = builder.AddAzureCosmosDB("cosmosdb")
    .RunAsPreviewEmulator();
var database = cosmosDb.AddCosmosDatabase("mydatabase");
database.AddContainer("mycontainer", "/id");

var apiService = builder.AddProject<Projects.DeployApp_ApiService>("apiservice")
    .WithHttpHealthCheck("/health")
    .WithReference(blob)
    .WithReference(database);

builder.AddProject<Projects.DeployApp_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
