using System.Collections;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Roo.Azure.Configuration.Common.Mapper
{
    /// <summary>
    /// Automatically maps one variable to another using type conventions and custom mappings.
    /// </summary>
    public interface IRooMapper
    {
        /// <summary>
        /// Maps a source object to a destination object of type TDestination.
        /// </summary>
        /// <typeparam name="TDestination">Destination type to map to.</typeparam>
        /// <param name="source">Source object to map from.</param>
        /// <returns></returns>
        TDestination? Map<TDestination>(object? source);
    }

    /// <summary>
    /// <inheritdoc cref="IRooMapper"/>
    /// </summary>
    public class RooMapper : IRooMapper
    {
        private readonly ConcurrentDictionary<(Type Source, Type Destination), Delegate> _mappings = new();
        private readonly ConcurrentDictionary<(Type Source, Type Destination), Delegate> _reverseMappings = new();
        private readonly ConcurrentDictionary<(Type Source, Type Destination), Delegate> _collectionMappings = new();
        private readonly ConcurrentDictionary<(Type Source, Type Destination), Delegate> _typedElementMappings = new();
        private static readonly ConcurrentDictionary<(Type Source, Type Destination), Delegate> _defaultConverters = new();

        /// <summary>
        /// Default converters for specific type mappings.
        /// </summary>
        static RooMapper()
        {
            //Default converters
            //from int
            _defaultConverters[(typeof(int), typeof(string))] = new Func<int, string>(x => x.ToString());
            //from string
            _defaultConverters[(typeof(string), typeof(int))] = new Func<string, int>(x => int.Parse(x));
            _defaultConverters[(typeof(string), typeof(double))] = new Func<string, double>(x => double.Parse(x));
            _defaultConverters[(typeof(string), typeof(long))] = new Func<string, long>(x => long.Parse(x));
            _defaultConverters[(typeof(string), typeof(DateTime))] = new Func<string, DateTime>(x => DateTime.Parse(x));
            //from double
            _defaultConverters[(typeof(double), typeof(string))] = new Func<double, string>(x => x.ToString());
            //from long
            _defaultConverters[(typeof(long), typeof(string))] = new Func<long, string>(x => x.ToString());
            //from dateTime
            _defaultConverters[(typeof(DateTime), typeof(string))] = new Func<DateTime, string>(x => x.ToString("o"));
        }

        /// <summary>
        /// Default initialization of <see cref="RooMapper"/>.
        /// </summary>
        public RooMapper() { }

        /// <summary>
        /// Initialize <see cref="RooMapper"/> with the currently configured <see cref="Profile"/>'s and their mappings.
        /// </summary>
        /// <param name="manager"></param>
        public RooMapper(RooMapperManager manager)
        {
            foreach (var profile in manager.GetProfiles())
            {
                AddProfile(profile);
            }
        }

        /// <summary>
        /// Stores a mapping profile in the configuration.
        /// </summary>
        /// <param name="profile"></param>
        public void AddProfile(Profile profile)
        {
            profile.Apply(this);
        }

        /// <summary>
        /// Creates a mapping expression for the source (first) and destination (second) types.
        /// </summary>
        /// <typeparam name="TSource">Type to map from.</typeparam>
        /// <typeparam name="TDestination">Type to map to.</typeparam>
        /// <returns></returns>
        public MappingExpression<TSource, TDestination> CreateMap<TSource, TDestination>() where TDestination : new()
        {
            RegisterAutoMap(typeof(TSource), typeof(TDestination));
            var expression = new MappingExpression<TSource, TDestination>(this);
            RegisterNestedSelfMaps(expression);
            return expression;
        }

        /// <summary>
        /// Creates a mapping function between source (first) and destination (second) types.
        /// </summary>
        /// <typeparam name="TSource">Type to map from.</typeparam>
        /// <typeparam name="TDestination">Type to map to.</typeparam>
        /// <param name="mapFunction"></param>
        public void CreateMap<TSource, TDestination>(Func<TSource, TDestination> mapFunction)
        {
            _mappings[(typeof(TSource), typeof(TDestination))] = mapFunction;
        }

        /// <summary>
        /// Adds a custom converter to the mapper for specific type mappings between source (first) and destiantion (second)<br/>
        /// Overrides default converters if they exist.
        /// </summary>
        /// <typeparam name="TSource">Type to map from.</typeparam>
        /// <typeparam name="TDestination">Type to map to.</typeparam>
        /// <param name="converter"></param>
        public void AddConverter<TSource, TDestination>(Func<TSource, TDestination> converter)
        {
            _mappings[(typeof(TSource), typeof(TDestination))] = new Func<object, TDestination>(src => converter((TSource)src));
        }

        /// <summary>
        /// Maps a source object to a destination type using registered mappings and/or default converters.
        /// </summary>
        /// <typeparam name="TDestination">Type to map to.</typeparam>
        /// <param name="source">Object to map from.</param>
        /// <returns></returns>
        public TDestination? Map<TDestination>(object? source)
        {
            if (source == null)
            {
                return default;
            }

            var sourceType = source.GetType();
            var destinationType = typeof(TDestination);

            //Handle IEnumerable<TSource> to IEnumerable<TDestination>
            if (sourceType.IsEnumerableType(out var sourceElementType) && destinationType.IsEnumerableType(out var destinationElementType))
            {
                var key = (sourceElementType, destinationElementType);
                if (!_mappings.ContainsKey(key!))
                {
                    throw new InvalidOperationException($"No mapping registered for {sourceElementType} -> {destinationElementType}.");
                }
                var collectionMapDelegate = GetOrAddCollectionMapping(sourceElementType!, destinationElementType!);
                var result = collectionMapDelegate.DynamicInvoke(source);
                if (destinationType.IsArray)
                {
                    if (result is IList list)
                    {
                        var array = Array.CreateInstance(destinationElementType!, list.Count);
                        list.CopyTo(array, 0);
                        return (TDestination)(object)array;
                    }
                    var toArrayMethod = result!.GetType().GetMethod("ToArray", Type.EmptyTypes);
                    if (toArrayMethod != null)
                    {
                        return (TDestination)toArrayMethod.Invoke(result, null)!;
                    }
                }
                if (destinationType.IsAssignableFrom(result!.GetType()))
                {
                    return (TDestination)result;
                }
                return (TDestination)Activator.CreateInstance(destinationType, result!)!;
            }

            //Try custom converter
            if (_mappings.TryGetValue((sourceType, destinationType), out var mappingFunction))
            {
                return (TDestination)((Func<object, object>)mappingFunction)(source);
            }
            //Try reverse mapping
            if (_reverseMappings.TryGetValue((sourceType, destinationType), out var reverseMappingFunction))
            {
                return (TDestination)((Func<object, object>)reverseMappingFunction)(source);
            }
            //Try default converter
            if (_defaultConverters.TryGetValue((sourceType, destinationType), out var defaultConverter))
            {
                return ((Func<object, TDestination>)(src => (TDestination)defaultConverter?.DynamicInvoke(src)!))(source);
            }
            //Try automatic convention-base reflection mapping
            if (TryAutoMap(sourceType, destinationType, out var autoMapFunction))
            {
                if (autoMapFunction != null)
                {
                    _mappings[(sourceType, destinationType)] = autoMapFunction;
                }
                return (TDestination)autoMapFunction?.DynamicInvoke(source)!;
            }
            return default;
        }

        internal void RegisterMappingDelegate<TSource, TDestination>(MappingExpression<TSource, TDestination> expression) where TDestination : new()
        {
            var customMaps = expression.GetCustomMapExpressions().ToDictionary(x => x.Key, x => x.Value);
            var function = MappingExpressionBuilder.BuildMapFunction<TSource, TDestination>(this, expression.GetCustomMapExpressions().ToDictionary(x => x.Key, x => x.Value), expression.GetIgnoredPropertyPaths().ToHashSet());
            Func<object, object> wrapper = src => function((TSource)src)!;
            _mappings[(typeof(TSource), typeof(TDestination))] = wrapper;
            _typedElementMappings[(typeof(TSource), typeof(TDestination))] = function;
            if (expression.ReverseMapDelegate != null)
            {
                Func<object, object> reverseWrapper = src => expression.ReverseMapDelegate((TDestination)src)!;
                _reverseMappings[(typeof(TDestination), typeof(TSource))] = reverseWrapper;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Func<TSource, TDestination> GetTypedMappingDelegate<TSource, TDestination>()
        {
            if (_typedElementMappings.TryGetValue((typeof(TSource), typeof(TDestination)), out var del))
            {
                return (Func<TSource, TDestination>)del;
            }
            if (_mappings.TryGetValue((typeof(TSource), typeof(TDestination)), out var objectDel))
            {
                var objectFunction = (Func<object, object>)objectDel;
                return source => (TDestination)objectFunction(source!);
            }
            throw new InvalidOperationException($"No mapping registered for {typeof(TSource)} -> {typeof(TDestination)}.");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Func<object, object> GetMappingDelegate(Type sourceType, Type destinationType)
        {
            if (sourceType == destinationType)
            {
                return x => x;
            }
            if (sourceType.IsEnumerableType(out var sourceElementType) && sourceElementType != null && destinationType.IsEnumerableType(out var destinationElementType) && destinationElementType != null)
            {
                var delegateCollection = GetOrAddCollectionMapping(sourceElementType, destinationElementType);
                return (Func<object, object>)delegateCollection;
            }
            if (!_mappings.TryGetValue((sourceType, destinationType), out var del))
            {
                throw new InvalidOperationException($"No mapping registered for {sourceType} -> {destinationType}.");
            }
            return (Func<object, object>)del;
        }

        internal void RegisterReverseMap<TSource, TDestination>(Func<TDestination, TSource> reverseMap)
        {
            _reverseMappings[(typeof(TDestination), typeof(TSource))] = new Func<object, object>(src => reverseMap((TDestination)src)!);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static object? ConvertValue(object? value, Type destinationType)
        {
            if (value == null)
            {
                return null;
            }

            var valueType = value.GetType();
            var underlyingType = Nullable.GetUnderlyingType(destinationType);
            if (underlyingType != null)
            {
                if (underlyingType == valueType)
                {
                    return value;
                }
                var converted = Convert.ChangeType(value, underlyingType);
                return Activator.CreateInstance(destinationType, converted);
            }

            if (destinationType.IsAssignableFrom(valueType))
            {
                return value;
            }

            try
            {
                return Convert.ChangeType(value, destinationType);
            }
            catch
            {
                return value;
            }
        }

        private Delegate GetOrAddCollectionMapping(Type sourceElementType, Type destinationElementType)
        {
            var key = (sourceElementType, destinationElementType);
            if (_collectionMappings.TryGetValue(key, out var existingMapping))
            {
                return existingMapping;
            }
            var method = typeof(RooMapper).GetMethod(nameof(BuildCollectionMap), BindingFlags.NonPublic | BindingFlags.Instance)!.MakeGenericMethod(sourceElementType, destinationElementType);
            var del = (Delegate)method.Invoke(this, null)!;
            _collectionMappings[key] = del;
            return del;
        }

        private Func<object, object> BuildCollectionMap<TSource, TDestination>()
        {
            var elementMap = GetTypedMappingDelegate<TSource, TDestination>();
            Func<IEnumerable<TSource>, List<TDestination>> typedMap = source =>
            {
                List<TDestination> result;
                if (source is ICollection<TSource> collection)
                {
                    result = new List<TDestination>(collection.Count);
                }
                else
                {
                    result = new List<TDestination>();
                }
                foreach (var item in source)
                {
                    result.Add(elementMap(item!));
                }
                return result;
            };
            return source =>
            {
                if (source is not IEnumerable<TSource> sourceEnum)
                {
                    throw new InvalidCastException($"Can't cast {source?.GetType()} to IEnumerable<{typeof(TSource).Name}>");
                }
                return typedMap(sourceEnum);
            };
        }

        /*private Func<object, object> BuildCollectionMap<TSource, TDestination>()
        {
            var elementMap = GetTypedMappingDelegate<TSource, TDestination>();
            return source =>
            {
                if (source is TSource[] array)
                {
                    var result = new TDestination[array.Length];
                    for (var i = 0; i < array.Length; i++)
                    {
                        result[i] = elementMap(array[i]);
                    }
                    return result.ToList();
                }
                else if (source is List<TSource> list)
                {
                    var result = new List<TDestination>(list.Count);
                    for (var i = 0; i < list.Count; i++)
                    {
                        result.Add(elementMap(list[i]));
                    }
                    return result;
                }
                else if (source is ICollection<TSource> collection)
                {
                    var result = new List<TDestination>(collection.Count);
                    foreach (var item in collection)
                    {
                        result.Add(elementMap(item));
                    }
                    return result;
                }
                else if (source is IEnumerable<TSource> enumerable)
                {
                    var result = new List<TDestination>();
                    foreach (var item in enumerable)
                    {
                        result.Add(elementMap(item));
                    }
                    return result;
                }
                else
                {
                    throw new InvalidCastException($"Can't cast {source?.GetType()} to IEnumerable<{typeof(TSource).Name}>.");
                }
            };
        }*/

        private void RegisterNestedSelfMaps<TSource, TDestination>(MappingExpression<TSource, TDestination> expression) where TDestination : new()
        {
            var customMapped = expression.GetCustomMappedProperties();
            var ignored = expression.GetIgnoredPropertyPaths();
            var customMapExpressions = expression.GetCustomMapExpressions();
            var nestedGroups = customMapped.Where(x => x.Contains('.')).GroupBy(x => x.Split('.')[0]);
            foreach (var group in nestedGroups)
            {
                var nestedPropertyName = group.Key;
                var nestedCustomMaps = customMapExpressions.Where(x => x.Key.StartsWith(nestedPropertyName + ".")).ToDictionary(x => x.Key.Substring(nestedPropertyName.Length + 1), x => x.Value);
                var nestedIgnored = ignored.Where(x => x.StartsWith(nestedPropertyName + ".")).Select(x => x.Substring(nestedPropertyName.Length + 1)).ToHashSet();
                var sourceNestedProperty = ReflectionCache.GetProperty(typeof(TSource), nestedPropertyName);
                var destinationNestedProperty = ReflectionCache.GetProperty(typeof(TDestination), nestedPropertyName);
                if (sourceNestedProperty != null && destinationNestedProperty != null && sourceNestedProperty.PropertyType == destinationNestedProperty.PropertyType)
                {
                    var key = (sourceNestedProperty.PropertyType, destinationNestedProperty.PropertyType);
                    if (!_mappings.ContainsKey(key))
                    {
                        var method = typeof(RooMapper).GetMethod(nameof(CreateMapWithCustomMaps), BindingFlags.Instance | BindingFlags.NonPublic)!.MakeGenericMethod(sourceNestedProperty.PropertyType, destinationNestedProperty.PropertyType);
                        method.Invoke(this, new object[] { nestedCustomMaps, nestedIgnored });
                    }
                }
            }
        }

        private MappingExpression<TSource, TDestination> CreateMapWithCustomMaps<TSource, TDestination>(Dictionary<string, LambdaExpression> customMaps, HashSet<string> ignoredProperties) where TDestination : new()
        {
            var expression = new MappingExpression<TSource, TDestination>(this);
            expression.SetCustomMapExpression(customMaps);
            foreach (var map in customMaps)
            {
                var property = ReflectionCache.GetProperty(typeof(TDestination), map.Key);
                if (property != null)
                {
                    var forMemberMethod = typeof(MappingExpression<TSource, TDestination>).GetMethods().First(x => x.Name == "ForMember" && x.GetParameters().Length == 2);
                    var parameterType = property.PropertyType;
                    var destinationParameter = Expression.Parameter(typeof(TDestination), "x");
                    var memberAccess = Expression.Property(destinationParameter, property);
                    var destinationLambda = Expression.Lambda(memberAccess, destinationParameter);
                    var memberConfigType = typeof(IPropertyConfigurationExpression<,,>).MakeGenericType(typeof(TSource), typeof(TDestination), parameterType);
                    var actionType = typeof(Action<>).MakeGenericType(memberConfigType);
                    var mapFromMethod = memberConfigType.GetMethod("MapFrom", new[] { map.Value.GetType() });
                    var memberConfigParameter = Expression.Parameter(memberConfigType, "cfg");
                    var callMapFrom = Expression.Call(memberConfigParameter, mapFromMethod!, Expression.Constant(map.Value, map.Value.GetType()));
                    var actionLambda = Expression.Lambda(actionType, callMapFrom, memberConfigParameter).Compile();
                    forMemberMethod.MakeGenericMethod(parameterType).Invoke(expression, new object[] { destinationLambda, actionLambda });
                }
            }
            foreach (var ignore in ignoredProperties)
            {
                var property = ReflectionCache.GetProperty(typeof(TDestination), ignore);
                if (property != null)
                {
                    var forMemberMethod = typeof(MappingExpression<TSource, TDestination>).GetMethods().First(x => x.Name == "ForMember" && x.GetParameters().Length == 2);
                    var parameterType = property.PropertyType;
                    var destinationParameter = Expression.Parameter(typeof(TDestination), "x");
                    var memberAccess = Expression.Property(destinationParameter, property);
                    var destinationLambda = Expression.Lambda(memberAccess, destinationParameter);
                    var memberConfigType = typeof(IPropertyConfigurationExpression<,,>).MakeGenericType(typeof(TSource), typeof(TDestination), parameterType);
                    var actionType = typeof(Action<>).MakeGenericType(memberConfigType);
                    var memberConfigParameter = Expression.Parameter(memberConfigType, "cfg");
                    var callMapFrom = Expression.Call(memberConfigParameter, memberConfigType.GetMethod("Ignore")!);
                    var actionLambda = Expression.Lambda(actionType, callMapFrom, memberConfigParameter).Compile();
                    forMemberMethod.MakeGenericMethod(parameterType).Invoke(expression, new object[] { destinationLambda, actionLambda });
                }
            }
            var function = MappingExpressionBuilder.BuildMapFunction<TSource, TDestination>(this, expression.GetCustomMapExpressions().ToDictionary(x => x.Key, x => x.Value), expression.GetIgnoredPropertyPaths().ToHashSet());
            Func<object, object> wrapper = src => function((TSource)src)!;
            _mappings[(typeof(TSource), typeof(TDestination))] = wrapper;
            return expression;
        }

        private void RegisterAutoMap(Type sourceType, Type destinationType, HashSet<(Type, Type)>? visited = null)
        {
            visited ??= new HashSet<(Type, Type)>();
            if (!visited.Add((sourceType, destinationType)))
            {
                return;
            }
            var sourceProperties = sourceType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var destinationProperties = destinationType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var destinationProperty in destinationProperties)
            {
                var sourceProperty = sourceProperties.FirstOrDefault(x => x.Name == destinationProperty.Name);
                if (sourceProperty == null)
                {
                    continue;
                }
                var sourcePropertyType = sourceProperty.PropertyType;
                var destinationPropertyType = destinationProperty.PropertyType;
                if (destinationPropertyType.IsEnumerableType(out var destinationElementType) && destinationElementType != null && destinationElementType != typeof(string) &&
                    sourcePropertyType.IsEnumerableType(out var sourceElementType) && sourceElementType != null && sourceElementType != typeof(string))
                {
                    CreateMapMethod(sourceElementType, destinationElementType);
                    RegisterAutoMap(sourceElementType, destinationElementType, visited);
                }
                else if (destinationPropertyType.IsComplexType() && sourcePropertyType.IsComplexType())
                {
                    CreateMapMethod(sourcePropertyType, destinationPropertyType);
                    RegisterAutoMap(sourcePropertyType, destinationPropertyType, visited);
                }
            }
        }

        private void CreateMapMethod(Type sourceType, Type destinationType)
        {
            var key = (sourceType, destinationType);
            if (!_mappings.ContainsKey(key))
            {
                var method = typeof(RooMapper).GetMethod(nameof(CreateMap), Type.EmptyTypes)!.MakeGenericMethod(sourceType, destinationType);
                method.Invoke(this, null);
            }
        }

        private static bool TryAutoMap(Type sourceType, Type destinationType, out Delegate? mapFunction)
        {
            var sourceProperties = sourceType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var destinationProperties = destinationType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var assignments = new List<(PropertyInfo Source, PropertyInfo Destination)>();

            foreach (var destinationProperty in destinationProperties)
            {
                //Skip readonly properties
                if (!destinationProperty.CanWrite)
                {
                    continue;
                }
                var sourceProperty = sourceProperties.FirstOrDefault(x => x.Name == destinationProperty.Name && x.CanRead && destinationProperty.PropertyType.IsAssignableFrom(x.PropertyType));
                if (sourceProperty != null)
                {
                    assignments.Add((sourceProperty, destinationProperty));
                }
            }

            if (assignments.Count == 0)
            {
                mapFunction = null;
                return false;
            }

            //Create mapping function dynamically
            mapFunction = new Func<object, object>(source =>
            {
                var destination = Activator.CreateInstance(destinationType)!;
                foreach (var (sourceProp, destinationProp) in assignments)
                {
                    var value = sourceProp.GetValue(source);
                    destinationProp.SetValue(destination, value);
                }
                return destination;
            });
            return true;
        }
    }
}
