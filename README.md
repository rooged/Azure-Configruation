# rooged.Azure# Roo.Azure.Configuration
A C#, .NET package designed to integrate easily with Azure services. Geared more towards a Kubernetes microservice architecture but usuable in any application.

## Common
Includes functionality to simplify application startup, service configuration, expand Azure App Insights logs and telemetry, Swagger setup, Redis integration, web app configuration, and certification validation.

### ApplicationConfiguration
Includes standard setup and configuration of a .NET application. Includes setup for custom header inclusion, Swagger, Azure App Insights, Redis, HttpContextAccessor, Options, Session, and header forwarding.
Use by including in Program.cs, add values to the StartupModel then pass it as the input.
If a web application and using AzureApplicationConfiguration, place **after** AzureApplicationConfiguration.

StartupModel:
[*Required*] AppInsightsConnectionString: Your connection string for Azure App Insights.
[*Required*] AppConfigurationConnectionString: Your connection string for Azure App Configuration.
[*Optional*] RedisConnectionString: Your connection string for Redis. Ignore to not configure Redis.
[*Required*] ChannelId: The name of the application, used for HTTP calls and App Insights logs.
[*Optional*] UseCertificateForwarding: Whether to use certificate forwarding middleware for authentication. Default is false.
[*Optional*] UseHeaderValidation: Whether to use header middleware for validating custom headers are in incoming HTTP calls. Default is true.
[*Optional*] UseExceptionFilter: Use exception filter for excpetion handling during HTTP calls. Default is true.
[*Optional*] AppConfigurationSections: Use to pull only specific sections from Azure App Configuration.
[*Optional*] AppConfigRefreshTriggerKey: The key-value in App Configuration to be refreshed whenever one is triggered. Default is the Sentinel settings key.
[*Optional*] SwaggerBasePath: Base path of API for Swagger setup.
[*Optional*] SwaggerCustomOperationId: Enable custom Operation Id generation for Swagger. Default is true.
[*Optional*] SwaggerDefinitionNames: Names for Swagger route instances (definition in Swagger UI). Should only add on to the default as it must start with 'private'. Set to null if you don't want to setup Swagger.
[*Optional*] SwaggerRoutePrefix: Add an optional prefix in Swaggers route.
[*Optional*] SwaggerFilterControllers: Controllers that will be hidden from Swagger UI. Case insensitive, searches using contains: *{searchWord}* Defaults to hide Redis APIs. Add onto default to hide other controllers or set to null to show Redis APIs.
[*Optional*] SwaggerFilterModels: Models that will be hidden from Swagger UI to improve responsiveness. Case insensitive, searches using contains: *{searchWord}*

Web application:
```
var builder = WebApplication.CreateBuilder(args);
var startupModel = new StartupModel()
{
  //Configure
}
builder.AddAzureAppConfiguration(startupModel);
builder.Services.ApplicationConfiguration(startupModel);
```

Console application:
```
var builder = HostBuilder.ConfigureServices((hostContext, services) =>
{
  var startupModel = new StartupModel()
  {
    //Configure
  }
  services.ApplicationConfiguration(startupModel);
};
```

### AzureApplicationConfiguration
Includes the continued setup and configuration of Azure App Configuration and Azure Feature Management for a .NET application.
Use by including in Program.cs **before** ApplicationConfiguration, add values to the StartupModel then pass it as the input.

Web application:
```
var builder = WebApplication.CreateBuilder(args);
var startupModel = new StartupModel()
{
  //Configure
}
builder.AddAzureAppConfiguration(startupModel);
```

### WebApplicationConfiguration
Includes standard setup and configuration of a .NET web application. Includes setup for Azure App Configuration, Swagger, HeaderMiddleware, ServiceExceptionFilter, Session, exception handler, routing, HttpsRedirection, and header forwarding.
Use by including in Program.cs, add values to the StartupModel then pass it as the input.

```
var builder = WebApplication.CreateBuilder(args);
var startupModel = new StartupModel()
{
  //Configure
}
builder.Services.ApplicationConfiguration(startupModel);

//Your continued app setup

var app = builder.CreateBuilder(args);
app.WebAppConfig(startupModel);
```

### Custom Headers, HeaderMiddleware, and HeaderService
The backbone of the log expansion and HTTP request wrapper in this package is built on the custom headers that are attached to all HTTP requests, context, logs, and telemetry.
The headers are set as defaults in HttpContext and can be accessed from anywhere in an application using HttpContextAccessor.
In non-web applications (i.e. applications with a HttpContext) create a default HttpContext in startup for the headers to be attached to.

**Headers:**
**channel-id:** The name of the application which can be queried in App Insights. Set from the Startup model.
**session-id:** A unique GUID that remains the same throughout a "Session" whether a user browser session or a batch jobs run.
**transaction-id:** A unique GUID specific to every request, a new one is generated every HTTP request.
[*Optional*] **user-info:**: A user info model that should only be set and used once a user has become authorized. Allows tracking of a user id, email, and/or username in HTTP requests.

The header middleware validates that every HTTP request has the 3 headers and will fail a request if it doesn't.
If a HttpContext.User has any security Claims then the middleware will also validate that the user-info header is set and is valid.

**IHeaderService:** Methods to get and check the validity of the currently set custom headers.

### RooLogger
Wrapper around ILogger that automatically attaches the custom headers and formats the message to enhance App Insights logs.
The channel-id header allows logs to be filtered based on channel-id name in App Insights.
The session-id and transaction-id headers allows a singular failure to be tracked across the entire stack from frontend to backend query.

```
public void LogInformation(HttpContext context, string? message = null, Exception? ex = null);
```

HttpContext is required as custom headers are set there. As mentioned before, create a DefaultHttpContext in non-web applications for the framework to attach headers to.

### RooTelemetryLogger


### Swagger


### Redis
