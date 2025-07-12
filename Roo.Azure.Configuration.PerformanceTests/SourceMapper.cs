using Roo.Azure.Configuration.Common.Mapper;

namespace Roo.Azure.Configuration.PerformanceTests
{
    public class SourceMapper : Profile
    {
        public SourceMapper()
        {
            CreateMap<SourceSub, DestinationSub>();
            CreateMap<SourceMetadata, DestinationMetadata>();
            CreateMap<Source, Destination>();
        }
    }
}
