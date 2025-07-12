namespace Roo.Azure.Configuration.PerformanceTests
{
    public class GenerateTestingData
    {
        public static List<Source> GenerateSourceDate(int count)
        {
            var result = new List<Source>();
            var random = new Random();
            for (var i = 0; i < count; i++)
            {
                var source = new Source()
                {
                    Id = i,
                    Name = $"Source name {i}",
                    Description = $"Description for source {i}",
                    IsActive = i % 2 == 0,
                    CreatedAt = DateTime.Now.AddDays(-i),
                    UpdatedAt = i % 3 == 0 ? DateTime.Now.AddDays(-i) : null,
                    Category = i % 2 == 0 ? "Category A" : "Category B",
                    Tags = new List<string> { $"Tag {i}A", $"Tag {i}B" },
                    AdditionalInfo = $"Additional info for source {i}",
                    Notes = $"Notes for source {i}",
                    Subs = new(),
                    Metadata = new()
                    {
                        Key = $"Key{i}",
                        Value = $"Value{i}",
                        Timestamp = DateTime.Now.AddMinutes(-i * 2),
                        Source = "Generated",
                        Version = i,
                        IpAddress = $"111.222.3.{i}{i}{i}",
                        UserAgent = "Mozilla/5.0",
                        SessionId = Guid.NewGuid().ToString(),
                        MetaTags = new()
                    }
                };
                var subCount = random.Next(0, 1000);
                for (var j = 0; j < subCount; j++)
                {
                    var sub = new SourceSub()
                    {
                        Id = j,
                        Name = $"Sub Name {j}",
                        Type = j % 2 == 0 ? "Car" : "House",
                        BuyDate = DateTime.Now.AddYears(-5).AddDays(i),
                        OwnerName = $"Owner {j}",
                        Genre = $"Genre {j + 1}",
                        Color = "Color",
                        Age = j % 10,
                        ListOfInts = new()
                    };
                    var listOfIntsCount = random.Next(0, 50);
                    for (var k = 0; k < listOfIntsCount; k++)
                    {
                        sub.ListOfInts.Add(random.Next(1, 1000));
                    }
                    source.Subs.Add(sub);
                }
                var metaTags = random.Next(0, 1000);
                for (var j = 0; j < metaTags; j++)
                {
                    source.Metadata.MetaTags.Add($"MetaTag {j}");
                }
                result.Add(source);
            }
            return result;
        }
    }
}
