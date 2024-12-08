var builder = DistributedApplication.CreateBuilder(args);

builder
	.AddProject<Projects.DevBetterWeb_Web>(nameof(Projects.DevBetterWeb_Web).Replace("_", string.Empty).ToLower());

builder.Build().Run();
