## Roo.Azure.Configuration
The Roo.Azure.Configuration packages are a C#, .NET SDK designed to simplify development and integrate with Azure services. This package is designed to allow new projects be setup with minimal configuration, standardize functionalities, improve developer productivity, and expand log information in Azure App Insights. This SDK is geared more towards a microservice architecture but can be used in any application that makes HTTP calls and/or uses Azure services. The SDK is split into 3 packages; Roo.Azure.Configuration.Common, Roo.Azure.Configuration.Common.Http, and Roo.Azure.Configuration.Common.Utilities:

**Common:** Header propagation, logger, Swagger setup, startup config, and simplified Azure Service Bus, Azure Feature Manager, and Redis.

**Common.Http:** HttpClient with custom headers, authentication (OAuth or Azure APIM), token storage, and object deserialization.

**Common.Utilities:** Ease of life functions such as type extensions and boiler plate's for Azure Blob and Azure File Share.

[Check the Wiki out for more information on the packages, functionality, and usage.](https://github.com/rooged/Azure-Configruation/wiki)

### Getting started
In your Program.cs, add values to the StartupModel then pass it to setup methods.

Web application:
```C#
var builder = WebApplication.CreateBuilder(args);
var startupModel = new StartupModel()
{
  //Set values
}
builder.AddAzureAppConfiguration(startupModel);
builder.Services.ApplicationConfiguration(startupModel);
var httpClients = new Dictionary<string, string>()
{
  { "YourClientName", "BaseUrlForTheClient" }
};
builder.Services.CommonPetsHttp(startupModel.ChannelId, httpClients);
```

Console application:
```C#
var builder = HostBuilder.ConfigureServices((hostContext, services) =>
{
  var startupModel = new StartupModel()
  {
    //Set values
  }
  services.ApplicationConfiguration(startupModel);
  var httpClients = new Dictionary<string, string>()
  {
    { "YourClientName", "BaseUrlForTheClient" }
  };
  services.CommonPetsHttp(startupModel.ChannelId, httpClients);
});
```

### Installing the packages
You can install the packages into a project using the [NuGet Package Manager](https://learn.microsoft.com/en-us/nuget/consume-packages/install-use-packages-visual-studio) in Visual Studio, just look up [Roo.Azure.Configuration.Common](https://www.nuget.org/packages?q=Roo.Azure.Configuration.Common&includeComputedFrameworks=true&prerel=true) and you'll be able to install them. If you prefer using the dotnet CLI then you can do so with:
```
dotnet add package Roo.Azure.Configuration.Common
dotnet add package Roo.Azure.Configuration.Common.Http
dotnet add package Roo.Azure.Configuration.Common.Utilities
```
Or you can add the project reference directly in your .csproj with (replace the x.x.x with your desired version):
```
<ItemGroup>
  <PackageReference Include="Roo.Azure.Configuration.Common" Version="x.x.x" />
  <PackageReference Include="Roo.Azure.Configuration.Common.Http" Version="x.x.x" />
  <PackageReference Include="Roo.Azure.Configuration.Common.Utilities" Version="x.x.x" />
</ItemGroup>
```

### Having trouble?
Feel free to reach out to me or open an issue.
