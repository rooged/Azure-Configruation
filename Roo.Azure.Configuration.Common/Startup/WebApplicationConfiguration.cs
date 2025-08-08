using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Roo.Azure.Configuration.Common.Middlewares;
using Roo.Azure.Configuration.Common.Models;

namespace Roo.Azure.Configuration.Common.Startup
{
    /// <summary>
    /// Application startup configuration for a web application. Use in conjuction with <see cref="ApplicationConfiguration"/>.
    /// </summary>
    public static class WebApplicationConfiguration
    {
        /// <summary>
        /// Configures setup for a web application, this should be used in conjunction with the application configuration.<br/>
        /// Sets up Swagger(OpenAPI), <see cref="HeaderMiddleware">Header validation</see>, Azure App Configuration.
        /// </summary>
        /// <param name="app"></param>
        /// <returns><see cref="WebApplication"/>> for further service configuration.</returns>
        public static WebApplication WebAppConfig(this WebApplication app, StartupModel model)
        {
            //Azure App Configuration
            app.UseAzureAppConfiguration();

            //Add session for user session management
            app.UseSession();

            //Add Swagger
            if (model.SwaggerDefinitionNames != null && model.SwaggerDefinitionNames.Count > 0)
            {
                app.UseSwagger(options =>
                {
                    if (!string.IsNullOrEmpty(model.SwaggerRoutePrefix))
                    {
                        options.RouteTemplate = $"/swagger/{model.SwaggerRoutePrefix}/{{documentName}}/swagger.json";
                    }
                    if (!string.IsNullOrEmpty(model.BasePath))
                    {
                        options.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
                        {
                            swaggerDoc.Servers = new List<OpenApiServer> { new() { Url = $"https://{httpReq.Host.Value}{model.BasePath}" } };
                        });
                    }
                });

                app.UseSwaggerUI(options =>
                {
                    foreach (var name in model.SwaggerDefinitionNames)
                    {
                        if (!string.IsNullOrEmpty(model.SwaggerRoutePrefix))
                        {
                            options.SwaggerEndpoint($"/swagger/{model.SwaggerRoutePrefix}/{name}/swagger.json", name);
                        }
                        else
                        {
                            options.SwaggerEndpoint($"/swagger/{name}/swagger.json", name);
                        }
                    }
                });
            }

            //Configure the HTTP request pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/error");
            }

            app.UseRouting();

            app.UseHttpsRedirection();

            if (model.UseCertificateForwarding)
            {
                app.UseCertificateForwarding();
            }
            app.UseForwardedHeaders();

            //Add middlewares
            if (model.UseExceptionFilter)
            {
                app.UseMiddleware<ServiceExceptionMiddleware>();
            }
            if (model.UseHeaderValidation)
            {
                app.UseMiddleware<HeaderMiddleware>();
            }

            if (model.UseCertificateForwarding)
            {
                app.MapControllers().RequireAuthorization();
            }
            else
            {
                app.MapControllers();
            }

            if (!string.IsNullOrEmpty(model.BasePath))
            {
                app.UsePathBase(model.BasePath);
            }

            return app;
        }
    }
}
