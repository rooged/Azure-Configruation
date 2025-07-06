using Newtonsoft.Json;

namespace Roo.Azure.Configuration.Common.Http.Models
{
    internal class AzureAdTokenResponse
    {
        [JsonProperty("token_type")]
        public string TokenType { get; set; } = "";

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonProperty("ext_expires_in")]
        public int ExtExpiresIn { get; set; }

        [JsonProperty("access_token")]
        public string AccessToken { get; set; } = "";
    }
}
