namespace Roo.Azure.Configuration.PerformanceTests
{
    public class Destination
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? Category { get; set; }
        public List<string>? Tags { get; set; }
        public string? AdditionalInfo { get; set; }
        public string? Notes { get; set; }
        public List<SourceSub>? Subs { get; set; }
        public SourceMetadata? Metadata { get; set; }
    }

    public class DestinationMetadata
    {
        public string? Key { get; set; }
        public string? Value { get; set; }
        public DateTime? Timestamp { get; set; }
        public string? Source { get; set; }
        public int Version { get; set; }
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public string? SessionId { get; set; }
        public List<string>? MetaTags { get; set; }
    }

    public class DestinationSub
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Type { get; set; }
        public DateTime? BuyDate { get; set; }
        public string? OwnerName { get; set; }
        public string? Genre { get; set; }
        public string? Color { get; set; }
        public int? Age { get; set; }
        public List<int>? ListOfInts { get; set; }
    }
}
