using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Roo.Azure.Configuration.Common.Logging;
using Roo.Azure.Configuration.Common.Middlewares;
using Roo.Azure.Configuration.Common.Models;
using Roo.Azure.Configuration.Common.Services;
using Roo.Azure.Configuration.Common.Telemetry;
using StackExchange.Redis;
using System.Collections.Immutable;

namespace Roo.Azure.Configuration.Common.Startup
{
    /// <summary>
    /// Application startup configuration for an application.
    /// </summary>
    public static class ApplicationConfiguration
    {
        /// <summary>
        /// Builds and adds services for a common app. Design for Kubernetes microservices but applicable in many contexts.
        /// </summary>
        /// <param name="services">IServiceCollection builder.</param>
        /// <param name="model">Startup model for configuration parameters.</param>
        /// <returns><see cref="IServiceCollection"/> for further service configuraiton.</returns>
        public static IServiceCollection AppConfig(this IServiceCollection services, StartupModel model)
        {
            //Add general services
            services.AddSession();
            services.AddHttpContextAccessor();
            services.AddOptions();
            if (model.UseExceptionFilter)
            {
                services.AddMvc(options =>
                {
                    //options.Filters.Add(new ExceptionFilterAttribute());
                });
            }

            //Add & configure Swagger
            if (!string.IsNullOrEmpty(model.ChannelId) && model.SwaggerDefinitionNames != null && model.SwaggerDefinitionNames.Count > 0)
            {
                services.AddSwaggerGen(options =>
                {
                    options.DocInclusionPredicate((docName, apiDesc) =>
                    {
                        if (docName.Equals(model.SwaggerDefinitionNames[0], StringComparison.CurrentCultureIgnoreCase))
                        {
                            return true;
                        }
                        else
                        {
                            if (apiDesc.GroupName != null)
                            {
                                return apiDesc.GroupName.Equals(docName, StringComparison.CurrentCultureIgnoreCase);
                            }
                            else
                            {
                                return false;
                            }
                        }
                    });

                    //Add basic information about the API
                    foreach (var name in model.SwaggerDefinitionNames)
                    {
                        options.SwaggerDoc(name, new()
                        {
                            Version = "v1",
                            Title = model.ChannelId + $" {name}",
                            Description = model.ChannelId + $" {name}"
                        });
                    }

                    if (model.SwaggerCustomOperationId)
                    {
                        options.CustomOperationIds(x => $"{x.ActionDescriptor.RouteValues["controller"]}_{x.HttpMethod}_{x.ActionDescriptor.RouteValues["action"]}");
                    }
                    options.OperationFilter<SwaggerHeader>();
                    SwaggerFilter.Controllers = model.SwaggerFilterControllers?.ToImmutableList();
                    SwaggerFilter.Models = model.SwaggerFilterModels?.ToImmutableList();
                    options.DocumentFilter<SwaggerFilter>();
                });
            }

            //Add initializer
            services.AddSingleton<ITelemetryInitializer, TelemetryInitializer>();

            //Add & configure App Insights
            var appInsightsOptions = new ApplicationInsightsServiceOptions()
            {
                ConnectionString = model.AppInsightsConnectionString
            };
            services.AddApplicationInsightsTelemetry(appInsightsOptions);
            services.AddApplicationInsightsKubernetesEnricher();
            services.Configure<LoggerFilterOptions>(options =>
            {
                //The Application Insights SDK adds a defualt logging filter that instructs ILogger to capture only Warning and more severe logs. Applications Insights requires an explicit override.
                //Log levels can also be configured using appsettings.json. For more information, see https://learn.microsoft.com/en-us/azure/azure-monitor/app/worker-service#ilogger-logs
                var rule = options.Rules.FirstOrDefault(x => x.ProviderName == "Microsoft.Extensions.Logging.ApplicationInsightsLoggerProvider");

                if (rule != null)
                {
                    options.Rules.Remove(rule);
                }
            });
            services.AddSingleton<ITelemetryInitializer>((_) => new CloudRoleNameTelemetryInitializer(model.ChannelId));

            //Add default & App Insights HTTP clients & header propagation
            services.AddTransient<HeaderPropagateMiddleware>();
            services.AddHttpClient(Constants.HTTPClientTelemetry).AddHttpMessageHandler<HeaderPropagateMiddleware>();
            services.AddHttpClient(Microsoft.Extensions.Options.Options.DefaultName).AddHttpMessageHandler<HeaderPropagateMiddleware>();

            //Add custom services
            services.TryAddSingleton<IHeaderService, HeaderService>();
            services.TryAddSingleton<IRooLogger, RooLogger>();
            //services.TryAddSingleton<IServicesException, ServiceException>();

            //Add & configure Redis cache
            if (model.RedisConnectionString != null)
            {
                var redisMultiplexer = ConnectionMultiplexer.Connect(model.RedisConnectionString);
                services.AddSingleton<IConnectionMultiplexer>(redisMultiplexer);
                services.AddScoped<IRedisService, RedisService>();
                services.AddDataProtection().PersistKeysToStackExchangeRedis(redisMultiplexer, model.ChannelId);
            }

            //Add certificate validation
            if (model.UseCertificateForwarding)
            {
                services.AddCertificateValidation();
            }

            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
                options.KnownNetworks.Clear();
                options.KnownProxies.Clear();
            });

            return services;
        }
    }
}
