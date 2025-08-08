namespace Roo.Azure.Configuration.Common.Models
{
    /// <summary>
    /// Used for all startup configuration extensions.
    /// All sensitive values should be pulled from Key Vault.
    /// Non-sensitive values can be retrieved from App Configuration.
    /// </summary>
    public class StartupModel
    {
        private string? _basePath = null;
        private string? _swaggerRoutePrefix = null;

        /// <summary>
        /// App Insights connection string.
        /// </summary>
        public string AppInsightsConnectionString { get; set; } = "";
        /// <summary>
        /// App Configuration connection string.
        /// </summary>
        public string AppConfigurationConnectionString { get; set; } = "";
        /// <summary>
        /// The name of this application, used for HTTP calls and App Insights logs.
        /// </summary>
        public string ChannelId { get; set; } = "";
        /// <summary>
        /// Use to pull only specific sections from Azure App Configuration.
        /// </summary>
        public List<string>? AppConfigurationSections { get; set; }
        /// <summary>
        /// The key-value in App Configuration to be refreshed whenever one is triggered.<br/>
        /// Default is the Sentinel settings key.
        /// </summary>
        public string? AppConfigRefreshTriggerKey { get; set; } = "Settings:Sentinel";
        /// <summary>
        /// The interval befor the App Configuration cache is refreshed. Default is 30 seconds.
        /// </summary>
        public TimeSpan AppConfigRefreshInterval { get; set; } = TimeSpan.FromSeconds(30);
        /// <summary>
        /// Base path of API setup.
        /// </summary>
        public string? BasePath
        {
            get => _basePath;
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    var temp = value.StartsWith('/') ? value : "/" + value;
                    _basePath = temp.EndsWith('/') ? temp[..^1] : temp;
                }
            }
        }
        /// <summary>
        /// Whether to use header middleware for validating custom headers are in incoming HTTP calls.
        /// Default is true.
        /// </summary>
        public bool UseHeaderValidation { get; set; } = true;
        /// <summary>
        /// Whether to use certificate forwarding middleware for authentication.
        /// Default is false.
        /// </summary>
        public bool UseCertificateForwarding { get; set; } = false;
        /// <summary>
        /// Use exception filter for excpetion handling during HTTP calls.
        /// Default is false.
        /// </summary>
        public bool UseExceptionFilter { get; set; } = false;
        /// <summary>
        /// Redis connection string.
        /// </summary>
        public string? RedisConnectionString { get; set; } = null;

        //Swagger specific values.
        /// <summary>
        /// Names for Swagger route instances (definition in Swagger UI).
        /// Should only add on to the default as it must start with 'private'. Each name needs to be unique.
        /// Set to null if you don't want to setup Swagger.
        /// </summary>
        public List<string>? SwaggerDefinitionNames { get; set; } = ["private"];
        /// <summary>
        /// Enable custom Operation Id generation for Swagger.
        /// Default is true.
        /// </summary>
        public bool SwaggerCustomOperationId { get; set; } = false;
        /// <summary>
        /// Add an optional prefix in Swaggers route.
        /// </summary>
        public string? SwaggerRoutePrefix
        {
            get => _swaggerRoutePrefix;
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    var temp = value.StartsWith('/') ? value.Substring(1) : value;
                    _swaggerRoutePrefix = temp.EndsWith('/') ? temp[..^1] : temp;
                }
            }
        }
        /// <summary>
        /// Controllers and Methods that will be hidden from Swagger UI.
        /// Case insensitive, searches using <see cref="string.Contains(string)"/>.
        /// Defaults to hide Redis APIs. Add onto default to hide other controllers or set to null to show Redis APIs.
        /// </summary>
        public List<string>? SwaggerFilterControllers { get; set; } = ["redis"];
        /// <summary>
        /// Models that will be hidden from Swagger UI to improve responsiveness.
        /// Case insensitive, searches using <see cref="string.Contains(string)"/>.
        /// </summary>
        public List<string>? SwaggerFilterModels { get; set; } = null;
    }
}
