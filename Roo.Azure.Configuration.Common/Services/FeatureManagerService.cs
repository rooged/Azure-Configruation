using Microsoft.FeatureManagement;

namespace Roo.Azure.Configuration.Common.Services
{
    /// <summary>
    /// Wrapper for Azure App Config Feature Manager methods.
    /// </summary>
    public interface IFeatureManagerService
    {
        /// <summary>
        /// Check if feature flag is enabled.
        /// </summary>
        /// <param name="name">Name of feature.</param>
        /// <returns>Whether feature is enabled.</returns>
        Task<bool> IsFeatureEnabled(string name);
        /// <summary>
        /// Check if feature flag is enabled with context used to evaluate whether a feature should be on or off.
        /// </summary>
        /// <typeparam name="T">Context of type T.</typeparam>
        /// <param name="name">Name of feature.</param>
        /// <param name="context">Name of feature.</param>
        /// <returns>Whether feature is enabled.</returns>
        Task<bool> IsFeatureEnabled<T>(string name, T context);
        /// <summary>
        /// Get all feature names from App Config.
        /// </summary>
        /// <returns>List of feature names.</returns>
        IAsyncEnumerable<string> GetFeatureNames();
    }

    /// <summary>
    /// Implementation of <see cref="IFeatureManagerService"/>
    /// </summary>
    public class FeatureManagerService : IFeatureManagerService
    {
        private readonly IFeatureManager _featureManager;

        /// <summary>
        /// Initialize <see cref="FeatureManagerService"/>
        /// </summary>
        /// <param name="featureManager"></param>
        public FeatureManagerService(IFeatureManager featureManager)
        {
            _featureManager = featureManager;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<bool> IsFeatureEnabled(string name)
        {
            return await _featureManager.IsEnabledAsync(name);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<bool> IsFeatureEnabled<T>(string name, T context)
        {
            return await _featureManager.IsEnabledAsync(name, context);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public IAsyncEnumerable<string> GetFeatureNames()
        {
            return _featureManager.GetFeatureNamesAsync();
        }
    }
}
