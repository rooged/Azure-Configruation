namespace Roo.Azure.Configuration.Common.Models
{
    /// <summary>
    /// Used for all startup configuration extensions.
    /// All sensitive values should be pulled from Key Vault.
    /// Non-sensitive values can be retrieved from App Configuration.
    /// </summary>
    public class StartupModel
    {
        /// <summary>
        /// App Insights connection string.
        /// </summary>
        public string AppInsightsConnectionString { get; set; } = "";
        /// <summary>
        /// App Configuration connection string.
        /// </summary>
        public string AppConfigurationConnectionString { get; set; } = "";
        /// <summary>
        /// Redis connection string.
        /// </summary>
        public string? RedisConnectionString { get; set; }
        /// <summary>
        /// Channel Id.
        /// </summary>
        public string ChannelId { get; set; } = "";
        /// <summary>
        /// Whether to use certificate forwarding middleware for authentication.
        /// Default is false.
        /// </summary>
        public bool UseCertificateForwarding { get; set; } = false;
        /// <summary>
        /// Whether to use header middleware for validating custom headers are in incoming HTTP calls.
        /// Default is true.
        /// </summary>
        public bool UseHeaderValidation { get; set; } = true;
        /// <summary>
        /// Use exception filter for excpetion handling during HTTP calls.
        /// </summary>
        public bool UseExceptionFilter { get; set; } = true;
        /// <summary>
        /// Use to pull only specific sections from Azure App Configuration.
        /// </summary>
        public List<string>? AppConfigurationSections { get; set; }
        /// <summary>
        /// Refresh App Configuration key.
        /// </summary>
        public string? RefreshTriggerKey { get; set; } = "Settings:Sentinel";


        //Swagger specific values.
        /// <summary>
        /// Base path of API for Swagger setup.
        /// </summary>
        public string SwaggerBasePath { get; set; } = "";
        /// <summary>
        /// Enable custom Operation Id generation for Swagger.
        /// Default is true.
        /// </summary>
        public bool SwaggerCustomOperationId { get; set; } = true;
        /// <summary>
        /// Names for Swagger route instances (definition in Swagger UI).
        /// Should only add on to the default as it must start with 'private'.
        /// Set to null if you don't want separate routing in Swagger.
        /// </summary>
        public List<string>? SwaggerDefinitionNames { get; set; } = ["private"];
        /// <summary>
        /// Add an optional prefix in Swaggers route.
        /// </summary>
        public string? SwaggerRoutePrefix { get; set; }
        /// <summary>
        /// Controllers that will be hidden from Swagger UI.
        /// Case insensitive, searches using contains: *{searchWord}*
        /// Defaults to hide Redis APIs. Add onto default to hide other controllers or set to null to show Redis APIs.
        /// </summary>
        public List<string>? SwaggerFilterControllers { get; set; } = ["redis"];
        /// <summary>
        /// Models that will be hidden from Swagger UI to improve responsiveness.
        /// Case insensitive, searches using contains: *{searchWord}*
        /// </summary>
        public List<string>? SwaggerFilterModels { get; set; }
    }
}
