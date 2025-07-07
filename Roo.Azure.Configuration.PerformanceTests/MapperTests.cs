using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Roo.Azure.Configuration.Common.Mapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Roo.Azure.Configuration.PerformanceTests
{
    public interface IMapperTests
    {
        void RunTests();
    }

    public class MapperTests : IMapperTests
    {
        private readonly IRooMapper _mapper;

        public MapperTests(IRooMapper mapper)
        {
            _mapper = mapper;
        }

        public void RunTests()
        {
            Console.WriteLine("Starting mapping tests.");

            Console.WriteLine("Single item mapping test, 10 times.");
            for (var i = 0; i < 10; i++)
            {
                Test(1);
            }

            Console.WriteLine("Group 1 mapping test.");
            var random = new Random();
            Test(random.Next(2, 10));

            Console.WriteLine("Group 2 mapping test.");
            Test(random.Next(25, 100));

            Console.WriteLine("Group 3 mapping test.");
            Test(random.Next(500, 1000));

            Console.WriteLine("Group 4 mapping test.");
            Test(random.Next(2000, 2500));

            Console.WriteLine("Group 5 mapping test.");
            Test(random.Next(9000, 10000));

            Console.WriteLine($"Ending mapping tests.");
        }

        private void Test(int testingCount)
        {
            GenerateTestingData generator = new();
            var generateDataBeginTime = DateTime.Now;
            Console.WriteLine($"Generating {testingCount} test data objects at {generateDataBeginTime}.");
            var testData = generator.GenerateSourceDate(testingCount);
            var generateDataEndTime = DateTime.Now;
            Console.WriteLine($"{testData.Count} test data objects generated at {generateDataEndTime}.");

            var mapperBeginTime = DateTime.Now;
            Console.WriteLine($"Starting Mapper tests at {mapperBeginTime}.");
            var mapperDestination = _mapper.Map<List<Destination>>(testData);
            var mapperEndTime = DateTime.Now;
            Console.WriteLine($"Ending Mapper tests at {mapperEndTime}.");

            var mapperDataValidationBeginTime = DateTime.Now;
            Console.WriteLine($"Starting Mapper data validation test at {mapperDataValidationBeginTime}.");
            var result = ValidateMap(testData, mapperDestination);
            Console.WriteLine($"Ending Mapper data validation test at {mapperDataValidationBeginTime} with data validity {result}.");

            Console.WriteLine($"Mapper mapping split time: {mapperEndTime - mapperBeginTime}.");
            Console.WriteLine();
        }

        private bool ValidateMap(List<Source> sources, List<Destination>? destinations)
        {
            if (destinations == null)
            {
                Console.WriteLine($"Mapper destination is null.");
                return false;
            }
            if (sources.Count > 0 && destinations.Count == 0)
            {
                Console.WriteLine($"Mapper destination count is 0.");
                return false;
            }
            if (sources.Count != destinations.Count)
            {
                Console.WriteLine($"Mapper source count {sources.Count} does not match destination count {destinations.Count}.");
                return false;
            }
            for (var i = 0; i < sources.Count - 1; i++)
            {
                var source = sources[i];
                var destination = destinations[i];
                ValidateInt(source.Id, destination.Id, nameof(source.Id));
                ValidateString(source.Name, destination.Name, nameof(source.Name));
                ValidateString(source.Description!, destination.Description, nameof(source.Description));
                ValidateBool(source.IsActive, destination.IsActive, nameof(source.IsActive));
                ValidateDateTime(source.CreatedAt, destination.CreatedAt, nameof(source.CreatedAt));
                ValidateDateTime(source.UpdatedAt, destination.UpdatedAt, nameof(source.UpdatedAt));
                ValidateString(source.Category!, destination.Category, nameof(source.Category));
                if (source.Tags != null)
                {
                    if (destination.Tags == null)
                    {
                        Console.WriteLine($"Mapper Tags property mismatch: source has {source.Tags.Count} tags, destination is null.");
                        continue;
                    }
                    if (source.Tags.Count > 0 && destination.Tags.Count == 0)
                    {
                        Console.WriteLine($"Mapper Tags property mismatch: source has {source.Tags.Count} tags, destination has 0 tags.");
                        continue;
                    }
                    if (source.Tags.Count != destination.Tags.Count)
                    {
                        Console.WriteLine($"Mapper Tags property mismatch: source has {source.Tags.Count} tags, destination has {destination.Tags.Count} tags.");
                        continue;
                    }
                    for (var j = 0; j < source.Tags.Count - 1; j++)
                    {
                        ValidateString(source.Tags[j], destination.Tags[j], $"{nameof(source.Tags)} index: {j}");
                    }
                }
                ValidateString(source.AdditionalInfo!, destination.AdditionalInfo, nameof(source.AdditionalInfo));
                ValidateString(source.Notes!, destination.Notes, nameof(source.Notes));
                if (source.Subs != null)
                {
                    if (destination.Subs == null)
                    {
                        Console.WriteLine($"Mapper Subs property mismatch: source has {source.Subs.Count} subs, destination is null.");
                        continue;
                    }
                    if (source.Subs.Count > 0 && destination.Subs.Count == 0)
                    {
                        Console.WriteLine($"Mapper Subs property mismatch: source has {source.Subs.Count} subs, destination has 0 subs.");
                        continue;
                    }
                    if (source.Subs.Count != destination.Subs.Count)
                    {
                        Console.WriteLine($"Mapper Subs property mismatch: source has {source.Subs.Count} subs, destination has {destination.Subs.Count} subs.");
                        continue;
                    }
                    for (var j = 0; j < source.Subs.Count - 1; j++)
                    {
                        var sourceSub = source.Subs[j];
                        var destinationSub = destination.Subs[j];
                        ValidateInt(source.Subs[j].Id, destination.Subs[j].Id, $"Source.Subs.Id index: {j}");
                        ValidateString(source.Subs[j].Name, destination.Subs[j].Name, $"Source.Subs.Name index: {j}");
                        ValidateString(source.Subs[j].Type!, destination.Subs[j].Type, $"Source.Subs.Type index: {j}");
                        ValidateDateTime(source.Subs[j].BuyDate, destination.Subs[j].BuyDate, $"Source.Subs.BuyDate index: {j}");
                        ValidateString(source.Subs[j].OwnerName!, destination.Subs[j].OwnerName, $"Source.Subs.OwnerName index: {j}");
                        ValidateString(source.Subs[j].Genre!, destination.Subs[j].Genre, $"Source.Subs.Genre index: {j}");
                        ValidateString(source.Subs[j].Color!, destination.Subs[j].Color, $"Source.Subs.Color index: {j}");
                        ValidateInt(source.Subs[j].Age, destination.Subs[j].Age, $"Source.Subs.Age index: {j}");
                        if (sourceSub.ListOfInts != null)
                        {
                            if (destinationSub.ListOfInts == null)
                            {
                                Console.WriteLine($"Mapper Subs.ListOfInts property mismatch: source has {sourceSub.ListOfInts.Count} subs, destination is null.");
                                continue;
                            }
                            if (sourceSub.ListOfInts.Count > 0 && destinationSub.ListOfInts.Count == 0)
                            {
                                Console.WriteLine($"Mapper Subs.ListOfInts property mismatch: source has {sourceSub.ListOfInts.Count}, destination has 0.");
                                continue;
                            }
                            if (sourceSub.ListOfInts.Count != destinationSub.ListOfInts.Count)
                            {
                                Console.WriteLine($"Mapper Subs.ListOfInts property mismatch: source has {sourceSub.ListOfInts.Count}, destination has {destinationSub.ListOfInts.Count}.");
                                continue;
                            }
                            for (var k = 0; k < sourceSub.ListOfInts.Count - 1; k++)
                            {
                                ValidateInt(sourceSub.ListOfInts[k], destinationSub.ListOfInts[k], $"Source.Subs.ListOfInts index: {j}");
                            }
                        }
                    }
                }
                if (source.Metadata != null)
                {
                    if (destination.Metadata == null)
                    {
                        Console.WriteLine($"Mapper Metadata property mismatch: source has metadata, destination is null.");
                        continue;
                    }
                    ValidateString(source.Metadata.Key!, destination.Metadata.Key, nameof(source.Metadata.Key));
                    ValidateString(source.Metadata.Value!, destination.Metadata.Value, nameof(source.Metadata.Value));
                    ValidateDateTime(source.Metadata.Timestamp, destination.Metadata.Timestamp, nameof(source.Metadata.Timestamp));
                    ValidateString(source.Metadata.Source!, destination.Metadata.Source, nameof(source.Metadata.Source));
                    ValidateInt(source.Metadata.Version, destination.Metadata.Version, nameof(source.Metadata.Version));
                    ValidateString(source.Metadata.IpAddress!, destination.Metadata.IpAddress, nameof(source.Metadata.IpAddress));
                    ValidateString(source.Metadata.UserAgent!, destination.Metadata.UserAgent, nameof(source.Metadata.UserAgent));
                    ValidateString(source.Metadata.SessionId!, destination.Metadata.SessionId, nameof(source.Metadata.SessionId));
                    if (source.Metadata.MetaTags != null)
                    {
                        if (destination.Metadata.MetaTags == null)
                        {
                            Console.WriteLine($"Mapper MetaTags property mismatch: source has MetaTags, destination is null.");
                            continue;
                        }
                        if (source.Metadata.MetaTags.Count > 0 && destination.Metadata.MetaTags.Count == 0)
                        {
                            Console.WriteLine($"Mapper MetaTags property mismatch: source has {source.Metadata.MetaTags.Count} tags, destination has 0 tags.");
                            continue;
                        }
                        if (source.Metadata.MetaTags.Count != destination.Metadata.MetaTags.Count)
                        {
                            Console.WriteLine($"Mapper MetaTags property mismatch: source has {source.Metadata.MetaTags.Count} tags, destination has {destination.Metadata.MetaTags.Count} tags.");
                            continue;
                        }
                        for (var j = 0; j < source.Metadata.MetaTags.Count - 1; j++)
                        {
                            ValidateString(source.Metadata.MetaTags[j], destination.Metadata.MetaTags[j], $"{nameof(source.Metadata.MetaTags)} index: {j}");
                        }
                    }
                }
            }
            return true;
        }

        private void ValidateString(string source, string? destination, string propertyName)
        {
            if (string.IsNullOrEmpty(destination))
            {
                Console.WriteLine($"Mapper string property {propertyName} mismatch: source {source}, destination is null.");
                return;
            }
            if (!source.Equals(destination))
            {
                Console.WriteLine($"Mapper string property {propertyName} mismatch: source {source}, destination {destination}.");
            }
        }

        private void ValidateInt(int source, int? destination, string propertyName)
        {
            if (destination == null)
            {
                Console.WriteLine($"Mapper int property {propertyName} mismatch: source {source}, destination is null.");
                return;
            }
            if (source != destination)
            {
                Console.WriteLine($"Mapper int property {propertyName} mismatch: source {source}, destination {destination}.");
            }
        }

        private void ValidateDateTime(DateTime? source, DateTime? destination, string propertyName)
        {
            if (source != null && destination == null)
            {
                Console.WriteLine($"Mapper DateTime property {propertyName} mismatch: source {source}, destination is null.");
                return;
            }
            if (source != destination)
            {
                Console.WriteLine($"Mapper DateTime property {propertyName} mismatch: source {source}, destination {destination}.");
            }
        }

        private void ValidateBool(bool source, bool? destination, string propertyName)
        {
            if (destination == null)
            {
                Console.WriteLine($"Mapper bool property {propertyName} mismatch: source {source}, destination is null.");
                return;
            }
            if (source != destination)
            {
                Console.WriteLine($"Mapper bool property {propertyName} mismatch: source {source}, destination {destination}.");
            }
        }
    }
}
