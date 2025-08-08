using Roo.Azure.Configuration.Common.Models;
using Roo.Azure.Configuration.Common.Startup;

var builder = WebApplication.CreateBuilder(args);

var startup = new StartupModel()
{
    AppConfigurationConnectionString = builder.Configuration.GetValue<string>("AppConfigConnectionString") ?? "",
    AppInsightsConnectionString = builder.Configuration.GetValue<string>("AppInsightsConnectionString") ?? "",
    ChannelId = builder.Configuration.GetValue<string>("ChannelId") ?? "",
    BasePath = "/api",
    SwaggerRoutePrefix = "backend",
    SwaggerDefinitionNames = ["private", "public", "example"]
};

builder.AddAzureAppConfiguration(startup);

builder.Services.AppConfig(startup);

var app = builder.Build();

app.WebAppConfig(startup);

app.Run();
