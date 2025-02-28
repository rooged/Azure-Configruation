using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;
using Microsoft.FeatureManagement.FeatureFilters;
using Roo.Azure.Configuration.Common.Models;
using Roo.Azure.Configuration.Common.Services;

namespace Roo.Azure.Configuration.Common.Startup
{
    /// <summary>
    /// Azure App Config and Feature Management setup and configuration.
    /// </summary>
    public static class AzureApplicationConfiguration
    {
        /// <summary>
        /// ConfigurationRefresher to refresh app config and feature flags manually.
        /// </summary>
        public static IConfigurationRefresher? ConfigurationRefresher { get; set; }

        public static WebApplicationBuilder AddAzureAppConfiguration(this WebApplicationBuilder builder, StartupModel model)
        {
            //Add App Configuration
            builder.Services.AddAzureAppConfiguration();

            //Add Feature Management
            builder.Services.AddFeatureManagement().AddFeatureFilter<TimeWindowFilter>();
            builder.Services.AddSingleton<IFeatureManagerService, FeatureManagerService>();

            var configSectionsLists = model.AppConfigurationSections != null ? model.AppConfigurationSections.ToList() : new List<string>();

            //Configure App Config and Feature Management
            builder.Configuration.AddAzureAppConfiguration(options =>
            {
                options.Connect(model.AppConfigurationConnectionString);
                model.AppConfigurationSections?.ForEach(x => options.Select(x));
                options.ConfigureRefresh(x => x.Register(model.RefreshTriggerKey, refreshAll: true));

                options.UseFeatureFlags(x =>
                {
                    //Load all feature flags with no label. To load specific feature flags and labels, set via FeatureFlagOptions.Select.
                    //Using the default cache expiration of 30 seconds but included the logic to change the time if needed.
                    x.SetRefreshInterval(TimeSpan.FromSeconds(30));
                });
                ConfigurationRefresher = options.GetRefresher();
            });

            return builder;
        }
    }
}
