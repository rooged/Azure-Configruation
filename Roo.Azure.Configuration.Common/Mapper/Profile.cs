using System.Linq.Expressions;

namespace Roo.Azure.Configuration.Common.Mapper
{
    /// <summary>
    /// Base class for mapping profiles.
    /// </summary>
    public abstract class Profile
    {
        private readonly List<Func<RooMapper, object>> _mappingActions = new();

        /// <summary>
        /// Initialize <see cref="Profile"/> and automatically call <see cref="Configure"/>.
        /// </summary>
        protected Profile()
        {
            Configure();
        }

        /// <summary>
        /// Override-able configuration to set mappings.
        /// </summary>
        public virtual void Configure() { }

        /// <summary>
        /// Applies currently queued mappings.
        /// </summary>
        /// <param name="mapper"></param>
        public void Apply(RooMapper mapper)
        {
            foreach (var action in _mappingActions)
            {
                action(mapper);
            }
        }

        /// <summary>
        /// <inheritdoc cref="RooMapper.CreateMap{TSource, TDestination}()"/>
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TDestination"></typeparam>
        /// <returns></returns>
        protected MappingExpression<TSource, TDestination> CreateMap<TSource, TDestination>() where TDestination : new()
        {
            var deferredExpression = new DeferredMappingExpression<TSource, TDestination>();
            _mappingActions.Add(x =>
            {
                var realExpression = x.CreateMap<TSource, TDestination>();
                deferredExpression.SetRealExpression(realExpression);
                x.RegisterMappingDelegate(realExpression);
                return realExpression;
            });
            //Return a proxy that will record further configuration, config isn't applied until Apply is called
            return deferredExpression;
        }
    }

    internal class DeferredMappingExpression<TSource, TDestination> : MappingExpression<TSource, TDestination>
    {
        private MappingExpression<TSource, TDestination>? _realExpression;
        private readonly List<Action<MappingExpression<TSource, TDestination>>> _pendingActions = new();

        public DeferredMappingExpression() : base(null!) { }

        public void SetRealExpression(MappingExpression<TSource, TDestination> realExpression)
        {
            _realExpression = realExpression;
            foreach (var action in _pendingActions)
            {
                action(realExpression);
            }
            _pendingActions.Clear();
        }

        public override MappingExpression<TSource, TDestination> ForMember<TMember>(Expression<Func<TDestination, TMember>> destinationMember, Action<IPropertyConfigurationExpression<TSource, TDestination, TMember>> memberOptions)
        {
            return GetOrQueue(x => x.ForMember(destinationMember, memberOptions));
        }

        public override MappingExpression<TSource, TDestination> ReverseMap()
        {
            return GetOrQueue(x => x.ReverseMap());
        }

        private MappingExpression<TSource, TDestination> GetOrQueue(Action<MappingExpression<TSource, TDestination>> action)
        {
            if (_realExpression == null)
            {
                _pendingActions.Add(action);
                return this;
            }
            action(_realExpression);
            return _realExpression;
        }
    }
}
