using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Roo.Azure.Configuration.Common.Http.AzureAdAuthentication;
using Roo.Azure.Configuration.Common.Logging;
using Roo.Azure.Configuration.Common.Middlewares;
using Roo.Azure.Configuration.Common.Services;

namespace Roo.Azure.Configuration.Common.Http
{
    /// <summary>
    /// Configure RooHttpClient in program startup.
    /// </summary>
    public static class RooHttpClientConfiguration
    {
        /// <summary>
        /// Configures a RooHttpClient and creates named HttpClients for every clientList item with the accompanying base URL.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="channelId">Channel Id of application.</param>
        /// <param name="httpClients">List of HttpClient names (as strings) and base URLs to configure.</param>
        /// <param name="useMemoryCache">Whether to use memory cache when storing tokens. Redis takes precedence if Redis connection string is passed in too. Memory cache will be used with Azure APIM if neither is passed in.</param>
        /// <param name="redisConnectionString">Connection string to Redis, controls whether to use Redis when storing tokens. Takes precedence over memory cache.</param>
        /// <param name=""></param>
        /// <returns></returns>
        public static IServiceCollection RooHttpClientConfig(this IServiceCollection services, string channelId, List<(string Name, string BaseUrl)> httpClients, string? redisConnectionString = null, bool useMemoryCache = false)
        {
            //Iterate through client list and create named client for each item
            foreach (var httpClient in httpClients)
            {
                if (string.IsNullOrEmpty(httpClient.Name) || string.IsNullOrEmpty(httpClient.BaseUrl))
                {
                    continue;
                }
                services.AddHttpClient(httpClient.Name, config =>
                {
                    config.BaseAddress = new Uri(httpClient.BaseUrl);
                }).AddHttpMessageHandler<HeaderPropagateMiddleware>();
            }
            return CreateHttpClient(services, channelId, redisConnectionString, useMemoryCache);
        }

        /// <summary>
        /// Configures a RooHttpClient and creates named HttpClients for every clientList item with the accompanying base URL.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="channelId">Channel Id of application.</param>
        /// <param name="httpClients">List of HttpClient names (as enums) and base URLs to configure.</param>
        /// <param name="useMemoryCache">Whether to use memory cache when storing tokens. Redis takes precedence if Redis connection string is passed in too. Memory cache will be used with Azure APIM if neither is passed in.</param>
        /// <param name="redisConnectionString">Connection string to Redis, controls whether to use Redis when storing tokens. Takes precedence over memory cache.</param>
        /// <param name=""></param>
        /// <returns></returns>
        public static IServiceCollection RooHttpClientConfig(this IServiceCollection services, string channelId, List<(Enum Name, string BaseUrl)> httpClients, string? redisConnectionString = null, bool useMemoryCache = false)
        {
            //Iterate through client list and create named client for each item
            foreach (var httpClient in httpClients)
            {
                if (string.IsNullOrEmpty(httpClient.Name.ToString()) || string.IsNullOrEmpty(httpClient.BaseUrl))
                {
                    continue;
                }
                services.AddHttpClient(httpClient.Name.ToString(), config =>
                {
                    config.BaseAddress = new Uri(httpClient.BaseUrl);
                }).AddHttpMessageHandler<HeaderPropagateMiddleware>();
            }
            return CreateHttpClient(services, channelId, redisConnectionString, useMemoryCache);
        }

        private static IServiceCollection CreateHttpClient(IServiceCollection services, string channelId, string? redisConnectionString = null, bool useMemoryCache = false)
        {
            //Add Azure Active Directory authentication service
            if (string.IsNullOrEmpty(redisConnectionString) && useMemoryCache == false)
            {
                services.TryAddSingleton<IAzureAdClientAssertion>(x =>
                {
                    var cache = x.GetRequiredService<IMemoryCache>();
                    return new AzureAdClientAssertion(cache);
                });
            }
            else
            {
                services.TryAddSingleton<IAzureAdClientAssertion>(x =>
                {
                    return new AzureAdClientAssertion();
                });
            }

            //Create RooHttpClient
            if (!string.IsNullOrEmpty(redisConnectionString))
            {
                services.AddScoped<IRooHttpClient>(x =>
                {
                    var httpContextAccessor = x.GetRequiredService<IHttpContextAccessor>();
                    var httpClientFactory = x.GetRequiredService<IHttpClientFactory>();
                    var logger = x.GetRequiredService<IRooLogger>();
                    var azureAdClientAssertion = x.GetRequiredService<IAzureAdClientAssertion>();
                    var redisService = x.GetRequiredService<IRedisService>();
                    return new RooHttpClient(channelId, httpContextAccessor, httpClientFactory, logger, azureAdClientAssertion, redisService);
                });
            }
            else if (useMemoryCache)
            {
                services.AddScoped<IRooHttpClient>(x =>
                {
                    var httpContextAccessor = x.GetRequiredService<IHttpContextAccessor>();
                    var httpClientFactory = x.GetRequiredService<IHttpClientFactory>();
                    var logger = x.GetRequiredService<IRooLogger>();
                    var azureAdClientAssertion = x.GetRequiredService<IAzureAdClientAssertion>();
                    var cache = x.GetRequiredService<IMemoryCache>();
                    return new RooHttpClient(channelId, httpContextAccessor, httpClientFactory, logger, azureAdClientAssertion, cache);
                });
            }
            else
            {
                services.AddScoped<IRooHttpClient>(x =>
                {
                    var httpContextAccessor = x.GetRequiredService<IHttpContextAccessor>();
                    var httpClientFactory = x.GetRequiredService<IHttpClientFactory>();
                    var logger = x.GetRequiredService<IRooLogger>();
                    var azureAdClientAssertion = x.GetRequiredService<IAzureAdClientAssertion>();
                    return new RooHttpClient(channelId, httpContextAccessor, httpClientFactory, logger, azureAdClientAssertion);
                });
            }
            return services;
        }
    }
}
