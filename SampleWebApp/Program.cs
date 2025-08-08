using Roo.Azure.Configuration.Common.Http;
using Roo.Azure.Configuration.Common.Models;
using Roo.Azure.Configuration.Common.Startup;
using SampleWebApp.Models.Enums;
using SampleWebApp.Services;

var builder = WebApplication.CreateBuilder(args);

var startup = new StartupModel()
{
    AppConfigurationConnectionString = builder.Configuration.GetValue<string>("AppConfigConnectionString") ?? "",
    AppInsightsConnectionString = builder.Configuration.GetValue<string>("AppInsightsConnectionString") ?? "",
    ChannelId = builder.Configuration.GetValue<string>("ChannelId") ?? "",
    BasePath = "/api",
    SwaggerRoutePrefix = "sample",
    SwaggerDefinitionNames = ["private", "public", "example"],
    //This hides the HiddenController and its methods from the Swagger documentation.
    SwaggerFilterControllers = ["hidden"],
    //This hides the HiddenModel class from the Swagger documentation.
    SwaggerFilterModels = ["hidden"]
};

//Adds Azure App Configuration
builder.AddAzureAppConfiguration(startup);

//Adds Azure App Insights, Swagger, RooLogger, Redis, Sesison, and other common services
builder.Services.AppConfig(startup);

//Adds RooHttpClient for HTTP client configuration
var httpClients = new List<(Enum Name, string BaseUrl)>
{
    (BaseUrls.ExampleBackend, builder.Configuration.GetValue<string>("SampleWebAppBackendBaseUrl") ?? "")
};
builder.Services.RooHttpClientConfig(startup.ChannelId, httpClients, useMemoryCache: true);

builder.Services.AddScoped<IExampleService, ExampleService>();

var app = builder.Build();

//Adds middleware for Azure App Config, Swagger, Session, Routing, Redirection, MapControllers, PathBase, and Header validation
app.WebAppConfig(startup);

app.Run();