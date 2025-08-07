using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Roo.Azure.Configuration.Common.Mapper;
using Roo.Azure.Configuration.PerformanceTests;

var builder = new HostApplicationBuilder();

var mapperCompileTimeStart = DateTime.Now;
Console.WriteLine($"Starting compile mapper profiles at {mapperCompileTimeStart}.");
var mappingConfig = new RooMapperManager(x =>
{
    x.AddProfile(new SourceMapper());
});
var mapper = mappingConfig.CreateMapper();
builder.Services.AddSingleton(mapper);
var mapperCompileTimeEnd = DateTime.Now;
Console.WriteLine($"Ending compile mapper profiles at {mapperCompileTimeEnd}.");

Console.WriteLine($"Compile split time: {mapperCompileTimeEnd - mapperCompileTimeStart}.");
Console.WriteLine();

builder.Services.AddSingleton<IMapperTests, MapperTests>();

var app = builder.Build();

var mapperTests = app.Services.GetRequiredService<IMapperTests>();
mapperTests.RunTests();