namespace Roo.Azure.Configuration.Common.Mapper
{
    /// <summary>
    /// Manager for mapping profiles.
    /// </summary>
    public class RooMapperManager
    {
        private readonly List<Profile> _profiles = new();

        /// <summary>
        /// Initialize <see cref="RooMapperManager"/>.
        /// </summary>
        /// <param name="configure"></param>
        public RooMapperManager(Action<RooMapperManager> configure)
        {
            configure(this);
        }

        /// <summary>
        /// Creates a new instance of <see cref="RooMapper"/> with the current configuration and profiles.
        /// </summary>
        /// <returns></returns>
        public IRooMapper CreateMapper()
        {
            var mapper = new RooMapper();
            foreach (var profile in _profiles)
            {
                profile.Apply(mapper);
            }
            return mapper;
        }

        /// <summary>
        /// Adds a profile to the manager.
        /// </summary>
        /// <param name="profile"></param>
        /// <returns></returns>
        public RooMapperManager AddProfile(Profile profile)
        {
            _profiles.Add(profile);
            return this;
        }

        internal IEnumerable<Profile> GetProfiles() => _profiles;
    }
}
