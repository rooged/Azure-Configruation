using System.Linq.Expressions;
using System.Reflection;

namespace Roo.Azure.Configuration.Common.Mapper
{
    /// <summary>
    /// Creates and stores mapping expressions between source and destination types.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TDestination"></typeparam>
    public class MappingExpression<TSource, TDestination>
    {
        private readonly List<Action<TSource, TDestination>> _propertyMaps = new();
        private readonly Dictionary<string, LambdaExpression> _customMapExpressions = new();
        private readonly HashSet<string> _ignoredPropertyPaths = new();
        private readonly HashSet<string> _customMappedProperties = new();
        private readonly RooMapper _mapper = new();

        /// <summary>
        /// Delegate for reverse mapping from destination to source type.
        /// </summary>
        public Func<TDestination, TSource>? ReverseMapDelegate { get; private set; }

        /// <summary>
        /// Event that is triggered when a reverse map function is registered.
        /// </summary>
        public event Action<Func<TDestination, TSource>?>? ReverseMapFunctionRegistered;

        internal IReadOnlyCollection<string> GetCustomMappedProperties() => _customMappedProperties;
        internal IReadOnlyCollection<string> GetIgnoredPropertyPaths() => _ignoredPropertyPaths;
        internal IReadOnlyDictionary<string, LambdaExpression> GetCustomMapExpressions() => _customMapExpressions;

        /// <summary>
        /// Initialize <see cref="MappingExpression{TSource, TDestination}"/> with an isntance of <see cref="RooMapper"/> for recursive mapping of nested properties.
        /// </summary>
        /// <param name="mapper"></param>
        public MappingExpression(RooMapper mapper)
        {
            _mapper = mapper;
        }

        /// <summary>
        /// Registers a mapping for a destination (first) from a source (second) property using a property, computed value, or constant.
        /// </summary>
        /// <typeparam name="TMember"></typeparam>
        /// <param name="destinationMember">Destination property to be mapped to.</param>
        /// <param name="memberOptions"></param>
        /// <returns>The mapping expression to use for converting the property.</returns>
        public virtual MappingExpression<TSource, TDestination> ForMember<TMember>(Expression<Func<TDestination, TMember>> destinationMember, Action<IPropertyConfigurationExpression<TSource, TDestination, TMember>> memberOptions)
        {
            var destinationMemberExpression = GetMemberExpression(destinationMember.Body);
            var path = GetPropertyPath(destinationMemberExpression);
            var destinationProperty = (PropertyInfo)destinationMemberExpression.Member;
            var memberConfig = new PropertyConfigurationExpression<TSource, TDestination, TMember>(destinationProperty, this, path);
            LambdaExpression? lambda = null;
            memberOptions(new LambdaCapturingMemberConfig<TSource, TDestination, TMember>(memberConfig, l => lambda = l));
            if (lambda != null)
            {
                if (path.Contains('.'))
                {
                    var firstDot = path.IndexOf('.');
                    var nestedProperty = path.Substring(0, firstDot);
                    var nestedPath = path.Substring(firstDot + 1);
                    var nestedSourceProperty = typeof(TSource).GetProperty(nestedProperty);
                    if (nestedSourceProperty == null)
                    {
                        throw new InvalidOperationException($"Property '{nestedProperty}' not found on source type '{typeof(TSource)}'.");
                    }
                    var nestedParameter = Expression.Parameter(nestedSourceProperty.PropertyType, "nestedSrc");
                    var newBody = new NestedParameterReplaceVisitor(lambda.Parameters[0], nestedProperty, nestedParameter).Visit(lambda.Body);
                    lambda = Expression.Lambda(newBody!, nestedParameter);
                }
                _customMapExpressions[path] = lambda;
            }
            _customMappedProperties.Add(path);
            return this;
        }

        /// <summary>
        /// Registers a reverse mapping function that maps from destination type to source type.
        /// </summary>
        /// <returns></returns>
        public virtual MappingExpression<TSource, TDestination> ReverseMap()
        {
            var destinationParameters = Expression.Parameter(typeof(TDestination), "dest");
            var sourceVariable = Expression.Variable(typeof(TSource), "src");
            var assignExpression = new List<Expression> { Expression.Assign(sourceVariable, Expression.New(typeof(TSource))) };
            foreach (var sourceProperty in typeof(TSource).GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(x => x.CanWrite))
            {
                var destinationProperty = typeof(TDestination).GetProperty(sourceProperty.Name, BindingFlags.Public | BindingFlags.Instance);
                if (destinationProperty == null || !destinationProperty.CanRead)
                {
                    continue;
                }
                var destinationPropertyExpression = Expression.Property(destinationParameters, destinationProperty);
                var sourcePropertyType = sourceProperty.PropertyType;
                var destinationPropertyType = destinationProperty.PropertyType;
                Expression valueExpression;
                if (MappingExtensions.IsComplexType(sourcePropertyType) && MappingExtensions.IsComplexType(destinationPropertyType) && sourcePropertyType != destinationPropertyType)
                {
                    var mapMethod = typeof(RooMapper).GetMethod(nameof(RooMapper.Map))!.MakeGenericMethod(sourcePropertyType);
                    valueExpression = Expression.Call(Expression.Constant(_mapper), mapMethod, Expression.Convert(destinationPropertyExpression, typeof(object)));
                    valueExpression = Expression.Convert(valueExpression, sourcePropertyType);
                }
                else
                {
                    valueExpression = Expression.Convert(destinationPropertyExpression, sourcePropertyType);
                }
                assignExpression.Add(Expression.Assign(Expression.Property(sourceVariable, sourceProperty), valueExpression));
            }
            assignExpression.Add(sourceVariable);
            var body = Expression.Block(new[] { sourceVariable }, assignExpression);
            var lambda = Expression.Lambda<Func<TDestination, TSource>>(body, destinationParameters);
            ReverseMapDelegate = lambda.Compile();
            _mapper.RegisterReverseMap(ReverseMapDelegate);
            ReverseMapFunctionRegistered?.Invoke(ReverseMapDelegate);
            return this;
        }

        internal void SetCustomMapExpression(Dictionary<string, LambdaExpression> customMaps)
        {
            foreach (var map in customMaps)
            {
                _customMapExpressions[map.Key] = map.Value;
            }
        }

        internal void AddCustomMap(Action<TSource, TDestination> map, string propertyPath)
        {
            _propertyMaps.Add(map);
            _customMappedProperties.Add(propertyPath);
        }

        internal void IgnoreProperty(string propertyPath)
        {
            _ignoredPropertyPaths.Add(propertyPath);
        }

        private static string GetPropertyPath(MemberExpression expression)
        {
            var path = new List<string>();
            while (expression != null)
            {
                path.Insert(0, expression.Member.Name);
                if (expression.Expression is MemberExpression parent)
                {
                    expression = parent;
                }
                else
                {
                    break;
                }
            }
            return string.Join(".", path);
        }

        private static MemberExpression GetMemberExpression(Expression expression)
        {
            if (expression is MemberExpression memberExpression)
            {
                return memberExpression;
            }
            if (expression is UnaryExpression unary && unary.Operand is MemberExpression memberOperand)
            {
                return memberOperand;
            }
            throw new ArgumentException("Expression is not a member access.", nameof(expression));
        }

        [Obsolete("Don't use.")]
        private void MapProperties(object? source, object? destination, string? parentPath = null, string? collectionParentPath = null)
        {
            var sourceProperties = source?.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var destinationProperties = destination?.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            if (destinationProperties == null)
            {
                return;
            }
            foreach (var destinationProperty in destinationProperties)
            {
                var destinationPropertyName = destinationProperty.Name;
                var fullPath = string.IsNullOrEmpty(parentPath) ? destinationPropertyName : $"{parentPath}.{destinationPropertyName}";
                if (_ignoredPropertyPaths.Contains(fullPath))
                {
                    continue;
                }
                var destinationPropertyType = destinationProperty.PropertyType;
                if (_ignoredPropertyPaths.Any(x => x.StartsWith(fullPath + ".")) && (MappingExtensions.IsComplexType(destinationPropertyType) || MappingExtensions.IsCollectionType(destinationPropertyType, out _)))
                {
                    continue;
                }
                if (_customMappedProperties.Contains(fullPath) || (collectionParentPath != null && _customMappedProperties.Contains($"{collectionParentPath}.{destinationPropertyName}")))
                {
                    continue;
                }
                if (_propertyMaps.Any(x => x.Method.GetParameters()[1].Name == destinationPropertyName || x.Method.GetParameters()[1].Name == fullPath))
                {
                    continue;
                }
                if (!destinationProperty.CanWrite)
                {
                    continue;
                }
                var value = GetNestedPropertyValue(source, fullPath);
                if (value == null && (parentPath == null || !fullPath.Contains('.')))
                {
                    var sourceProperty = source?.GetType().GetProperty(destinationPropertyName);
                    if (sourceProperty != null && sourceProperty.CanRead)
                    {
                        value = sourceProperty.GetValue(source);
                    }
                }
                //Handle collections of nested properties
                if (value != null && MappingExtensions.IsCollectionType(destinationPropertyType, out var destinationElementType) && MappingExtensions.IsCollectionType(value.GetType(), out var sourceElementType))
                {
                    var mapDelegate = _mapper.GetMappingDelegate(value.GetType(), destinationPropertyType);
                    var mappedValue = mapDelegate(value);
                    destinationProperty.SetValue(destination, RooMapper.ConvertValue(mappedValue, destinationPropertyType));
                    continue;
                }
                //Handle complex objects
                if (MappingExtensions.IsComplexType(destinationPropertyType))
                {
                    if (value == null)
                    {
                        destinationProperty.SetValue(destination, null);
                    }
                    else
                    {
                        var mapDelegate = _mapper.GetMappingDelegate(value.GetType(), destinationPropertyType);
                        var mappedValue = mapDelegate(value);
                        destinationProperty.SetValue(destination, RooMapper.ConvertValue(mappedValue, destinationPropertyType));
                    }
                    continue;
                }
                destinationProperty.SetValue(destination, RooMapper.ConvertValue(value, destinationPropertyType));
            }
        }

        private static object? GetNestedPropertyValue(object? obj, string propertyPath)
        {
            if (obj == null)
            {
                return null;
            }
            var parts = propertyPath.Split('.');
            var current = obj;
            foreach (var part in parts)
            {
                if (current == null)
                {
                    return null;
                }
                var property = current.GetType().GetProperty(part);
                if (property == null)
                {
                    return null;
                }
                current = property.GetValue(current);
            }
            return current;
        }
    }

    internal class LambdaCapturingMemberConfig<TSource, TDestination, TMember> : IPropertyConfigurationExpression<TSource, TDestination, TMember>
    {
        private readonly IPropertyConfigurationExpression<TSource, TDestination, TMember> _inner;
        private readonly Action<LambdaExpression> _onMapFrom;

        public LambdaCapturingMemberConfig(IPropertyConfigurationExpression<TSource, TDestination, TMember> inner, Action<LambdaExpression> onMapFrom)
        {
            _inner = inner;
            _onMapFrom = onMapFrom;
        }

        public void Ignore() => _inner.Ignore();

        public void MapFrom(Expression<Func<TSource, TMember>> source)
        {
            _onMapFrom(source);
            _inner.MapFrom(source);
        }

        public void MapFrom(TMember value) => _inner.MapFrom(value);
    }

    internal class NestedParameterReplaceVisitor : ExpressionVisitor
    {
        private readonly ParameterExpression _rootParameter;
        private readonly string _nestedProperty;
        private readonly ParameterExpression _nestedParameter;

        public NestedParameterReplaceVisitor(ParameterExpression rootParameter, string nestedProperty, ParameterExpression nestedParameter)
        {
            _rootParameter = rootParameter;
            _nestedProperty = nestedProperty;
            _nestedParameter = nestedParameter;
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            if (node.Expression == _rootParameter && node.Member.Name == _nestedProperty)
            {
                return _nestedParameter;
            }
            return base.VisitMember(node);
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return base.VisitParameter(node);
        }
    }
}
