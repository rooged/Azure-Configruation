using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Roo.Azure.Configuration.Common.Http;
using Roo.Azure.Configuration.Common.Models;
using Roo.Azure.Configuration.Common.Startup;

var builder = new HostApplicationBuilder();

//Set appsettings.json file property "Copy to Output Directory" as either Always or If Newer
var configuration = new ConfigurationBuilder().AddUserSecrets<Program>().AddJsonFile("appsettings.json", optional: false, reloadOnChange: true).Build();

var startup = new StartupModel()
{
    AppConfigurationConnectionString = configuration.GetValue<string>("AppConfigConnectionString") ?? "",
    AppInsightsConnectionString = configuration.GetValue<string>("AppInsightsConnectionString") ?? "",
    ChannelId = configuration.GetValue<string>("ChannelId") ?? ""
};

//Add Azure App Configuration
builder.AddAzureAppConfiguration(startup);

//Add Azure App Insights, RooLogger, Session, and non-web services
builder.Services.AppConfig(startup, false);

//Adds RooHttpClient for HTTP client configuration
var httpClients = new List<(string Name, string BaseUrl)>
{
    ("ExampleBackend", builder.Configuration.GetValue<string>("SampleWebAppBackendBaseUrl") ?? "")
};
builder.Services.RooHttpClientConfig(startup.ChannelId, httpClients);

var app = builder.Build();

app.Run();