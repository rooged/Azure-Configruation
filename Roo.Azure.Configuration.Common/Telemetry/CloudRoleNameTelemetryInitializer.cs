using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;

namespace Roo.Azure.Configuration.Common.Telemetry
{
    /// <summary>
    /// Set custom name for App Insights telemetry.
    /// </summary>
    public class CloudRoleNameTelemetryInitializer : ITelemetryInitializer
    {
        private readonly string _cloudRoleName;

        /// <summary>
        /// Initializes a new instance of the <see cref="CloudRoleNameTelemetryInitializer"/> class.
        /// </summary>
        /// <param name="cloudRoleName"></param>
        public CloudRoleNameTelemetryInitializer(string cloudRoleName)
        {
            _cloudRoleName = cloudRoleName;
        }

        /// <summary>
        /// Set cloud role name in App Insights.
        /// </summary>
        /// <param name="telemetry"></param>
        public void Initialize(ITelemetry telemetry) => telemetry.Context.Cloud.RoleName = _cloudRoleName;
    }
}
