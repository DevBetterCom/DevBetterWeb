using DevBetterWeb.AppHost;

var builder = DistributedApplication.CreateBuilder(args);

builder.AddDevBetterWeb();

builder.Build().Run();
