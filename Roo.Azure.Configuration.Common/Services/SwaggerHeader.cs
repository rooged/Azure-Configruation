using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Roo.Azure.Configuration.Common.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Diagnostics.CodeAnalysis;

namespace Roo.Azure.Configuration.Common.Services
{
    /// <summary>
    /// Adds the custom headers to Swagger.
    /// </summary>
    public class SwaggerHeader : IOperationFilter
    {
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="SwaggerHeader"/> class.
        /// </summary>
        /// <param name="config"></param>
        /// <param name="httpContextAccessor"></param>
        public SwaggerHeader(IConfiguration config, IHttpContextAccessor httpContextAccessor)
        {
            _config = config;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Adds the custom headers to Swagger.
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="context"></param>
        [ExcludeFromCodeCoverage]
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
            {
                operation.Parameters = new List<OpenApiParameter>();
            }

            operation.Parameters.Add(new OpenApiParameter()
            {
                Name = Constants.SessionIdHeaderName,
                In = ParameterLocation.Header,
                Required = true,
                Schema = new OpenApiSchema
                {
                    Type = "string",
                    Default = new OpenApiString(_httpContextAccessor.HttpContext?.Session.GetString(Constants.SessionId) ?? _httpContextAccessor.HttpContext?.Session.Id ?? Guid.NewGuid().ToString())
                }
            });
            operation.Parameters.Add(new OpenApiParameter()
            {
                Name = Constants.TransactionIdHeaderName,
                In = ParameterLocation.Header,
                Required = true,
                Schema = new OpenApiSchema
                {
                    Type = "string",
                    Default = new OpenApiString(Guid.NewGuid().ToString("N"))
                }
            });
            operation.Parameters.Add(new OpenApiParameter()
            {
                Name = Constants.ChannelIdHeaderName,
                In = ParameterLocation.Header,
                Required = true,
                Schema = new OpenApiSchema
                {
                    Type = "string",
                    Default = new OpenApiString(_config.GetValue<string>(Constants.ChannelId))
                }
            });
            operation.Parameters.Add(new OpenApiParameter()
            {
                Name = Constants.UserInfoHeaderName,
                In = ParameterLocation.Header,
                Required = false,
                Schema = new OpenApiSchema
                {
                    Type = "object",
                    Format = "text/json",
                    Properties = MakeSchemaProps<UserInfo>()
                }
            });
        }

        /// <summary>
        /// Create a schema for object of type T.
        /// </summary>
        /// <typeparam name="T">The type to make the schema of.</typeparam>
        /// <returns>Dictionary schema of type T.</returns>
        private static Dictionary<string, OpenApiSchema> MakeSchemaProps<T>()
        {
            var schema = new Dictionary<string, OpenApiSchema>();
            var modelType = typeof(T);
            var modelProps = modelType.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            foreach (var property in modelProps)
            {
                schema.Add(property.Name, new OpenApiSchema()
                {
                    Type = "string"
                });
            }
            return schema;
        }
    }
}
