using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Immutable;

namespace Roo.Azure.Configuration.UnitTests
{
    public class SwaggerFilterTests
    {
        //Common
        private string controller = "TestController";
        private string schema = "TestModel";

        [Test]
        public void ApplyPathsFilter_Verify()
        {
            //Arrange
            var schemaGenerator = new Mock<ISchemaGenerator>();
            var filterContext = new DocumentFilterContext(new List<ApiDescription>(), schemaGenerator.Object, null);
            var swaggerFilter = new SwaggerFilter();
            var swaggerDoc = new OpenApiDocument
            {
                Paths = new OpenApiPaths
                {
                    ["test/test"] = new OpenApiPathItem(),
                    ["test2/test"] = new OpenApiPathItem(),
                    [$"test3/{controller}"] = new OpenApiPathItem(),
                    ["test4/test"] = new OpenApiPathItem(),
                    [$"{controller}5/test"] = new OpenApiPathItem()
                },
                Components = new OpenApiComponents
                {
                    Schemas =
                    {
                        ["test"] = new OpenApiSchema(),
                        ["test2"] = new OpenApiSchema(),
                        [schema] = new OpenApiSchema(),
                        ["test4"] = new OpenApiSchema(),
                    }
                }
            };
            SwaggerFilter.Controllers = ImmutableList.Create(controller);
            SwaggerFilter.Models = ImmutableList.Create(schema);

            //Act
            ((IDocumentFilter)swaggerFilter).Apply(swaggerDoc, filterContext);
            var paths = swaggerDoc.Paths;
            var schemas = swaggerDoc.Components.Schemas;

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(paths, Is.Not.Null);
                Assert.That(paths.Where(x => x.Key == controller).Count, Is.EqualTo(0));
                Assert.That(schemas, Is.Not.Null);
                Assert.That(schemas.Where(x => x.Key == schema).Count, Is.EqualTo(0));
            });
        }
    }
}