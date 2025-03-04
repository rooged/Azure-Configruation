using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Immutable;

namespace Roo.Azure.Configuration.Common.Services
{
    /// <summary>
    /// Filters controllers and models from Swagger UI.<br/>
    /// Update StartupModel SwaggerFilterControllers and SwaggerFilterModels to add/remove.
    /// </summary>
    public class SwaggerFilter : IDocumentFilter
    {
        /// <summary>
        /// Controllers to hide from Swagger UI.
        /// </summary>
        public static ImmutableList<string>? Controllers = null;
        /// <summary>
        /// Models to hide from Swagger UI.
        /// </summary>
        public static ImmutableList<string>? Models = null;

        /// <summary>
        /// Filter controlelrs and models from SwaggerUI.
        /// </summary>
        /// <param name="swaggerDoc"></param>
        /// <param name="context"></param>
        /// <exception cref="NotImplementedException"></exception>
        void IDocumentFilter.Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            if (Controllers != null)
            {
                swaggerDoc.Paths.Where(x => Controllers.Any(y => x.Key.Contains(y, StringComparison.OrdinalIgnoreCase))).ToList().ForEach(x => swaggerDoc.Paths.Remove(x.Key));
            }

            if (Models != null)
            {
                swaggerDoc.Components.Schemas.Where(x => Models.Any(y => x.Key.Contains(y, StringComparison.OrdinalIgnoreCase))).ToList().ForEach(x => swaggerDoc.Components.Schemas.Remove(x.Key));
            }

            //use to filter based on swagger docs: swaggerDoc.Info.Title.Equals("");
        }
    }
}
