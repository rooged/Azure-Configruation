using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Roo.Azure.Configuration.Common.Mapper;

namespace Roo.Azure.Configuration.PerformanceTests
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var host = new HostBuilder().ConfigureServices((hostContext, services) =>
            {
                var mapperCompileTimeStart = DateTime.Now;
                Console.WriteLine($"Starting compile mapper profiles at {mapperCompileTimeStart}.");
                var mappingConfig = new RooMapperManager(x =>
                {
                    x.AddProfile(new SourceMapper());
                });
                var mapper = mappingConfig.CreateMapper();
                services.AddSingleton(mapper);
                var mapperCompileTimeEnd = DateTime.Now;
                Console.WriteLine($"Ending compile mapper profiles at {mapperCompileTimeEnd}.");

                Console.WriteLine($"Compile split time: {mapperCompileTimeEnd - mapperCompileTimeStart}.");
                Console.WriteLine();

                services.AddSingleton<IMapperTests, MapperTests>();
            }).Build();

            var mapperTests = host.Services.GetRequiredService<IMapperTests>();
            mapperTests.RunTests();
        }
    }
}
