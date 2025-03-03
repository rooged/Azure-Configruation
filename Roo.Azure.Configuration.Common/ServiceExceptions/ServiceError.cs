using Newtonsoft.Json;
using System.Text;

namespace Roo.Azure.Configuration.Common.ServiceExceptions
{
    /// <summary>
    /// Custom error model to relate the custom headers to the error to create better exception logs.
    /// </summary>
    public class ServiceError
    {
        /// <summary>
        /// Initialize <see cref="ServiceError"/>.
        /// </summary>
        /// <param name="code">Custom error code.</param>
        /// <param name="message">Message to include with the error.</param>
        /// <param name="details">Additional details about the error.</param>
        /// <param name="transactionId">TransactionId associated with the error.</param>
        public ServiceError(ErrorCode code, string? message = null, Dictionary<string, string>? details = null, string? transactionId = null)
        {
            Code = code;
            CodeName = Enum.GetName(typeof(ErrorCode), code) ?? $"Error finding name of error code {code}.";
            Message = message;
            Details = details;
            TransactionId = transactionId;
        }

        /// <summary>
        /// Custom error code.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public ErrorCode Code { get; set; }

        /// <summary>
        /// Name associated with the custom error code.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string CodeName { get; set; }

        /// <summary>
        /// Message associated with the error.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? Message { get; set; }

        /// <summary>
        /// Additional details about the error.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, string>? Details { get; set; }

        /// <summary>
        /// TransactionId associated with the error.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? TransactionId { get; set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Error Code:{Code}");
            sb.AppendLine($"Error Code Name:{CodeName}");
            sb.AppendLine($"Message:{Message}");
            sb.AppendLine($"Error Details:{Details}");
            if (Details != null)
            {
                foreach (var detail in Details.Keys)
                {
                    sb.AppendLine($"{detail}: {Details[detail]}");
                }
            }
            sb.AppendLine($"Transaction Id:{TransactionId}");
            return sb.ToString();
        }
    }
}
