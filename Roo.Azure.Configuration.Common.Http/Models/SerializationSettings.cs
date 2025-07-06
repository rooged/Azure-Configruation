using Newtonsoft.Json;
using System.Text.Json;

namespace Roo.Azure.Configuration.Common.Http.Models
{
    /// <summary>
    /// Use to edit settings for serialilzing HTTP responses
    /// </summary>
    public class SerializationSettings
    {
        /// <summary>
        /// Read response as a string when serializing
        /// </summary>
        public bool ReadAsString { get; set; } = false;

        /// <summary>
        /// Use default System.Text.Json serialization settings. Default settings:<br/>
        /// <list type="bullet">
        /// <item>ContractResolver = new CamelCasePropertyNamesContractResolver()</item>
        /// <item>DateTimeZoneHandling = DateTimeZoneHandling.Local</item>
        /// <item>NullValueHandling = NullValueHandling.Ignore</item>
        /// </list>
        /// </summary>
        public bool UseDefaultSerializationSettings { get; set; } = false;

        /// <summary>
        /// Newtonsoft.Json serializer settings.<br/>
        /// Will override default settings if UseDefaultSerializationSettings is set to true.
        /// </summary>
        public JsonSerializerSettings? JsonSerializerSettings { get; set; } = null;

        /// <summary>
        /// System.Text.Json serializer settings.<br/>
        /// Will override default settings if UseDefaultSerializationSettings is set to true.
        /// </summary>
        public JsonSerializerOptions? JsonSerializerOptions { get; set; } = null;
    }
}
