using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
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
                if (!string.IsNullOrEmpty(model.SwaggerRoutePrefix))
                {
                    app.UseSwagger(options =>
                    {
                        options.RouteTemplate = "/swagger/" + model.SwaggerRoutePrefix + "/{documentVersion}/{documentName}/swagger.json";
                        if (!string.IsNullOrEmpty(model.SwaggerBasePath))
                        {
                            options.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
                            {
                                swaggerDoc.Servers = [new() { Url = $"https://{httpReq.Host.Value}{model.SwaggerBasePath}" }];
                            });
                        }
                    });
                }
                else
                {
                    app.UseSwagger();
                }

                app.UseSwaggerUI(options =>
                {
                    if (!string.IsNullOrEmpty(model.SwaggerRoutePrefix))
                    {
                        foreach (var definition in model.SwaggerDefinitionNames)
                        {
                            options.SwaggerEndpoint($"/swagger/{model.SwaggerRoutePrefix}/v1/{definition}/swagger.json", definition);
                        }
                    }
                    else
                    {
                        foreach (var definition in model.SwaggerDefinitionNames)
                        {
                            options.SwaggerEndpoint($"/swagger/v1/{definition}/swagger.json", definition);
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

            return app;
        }
    }
}
