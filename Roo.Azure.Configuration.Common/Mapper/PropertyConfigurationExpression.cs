using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Roo.Azure.Configuration.Common.Mapper
{
    /// <summary>
    /// Manages property configuration for mapping expressions.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TDestination"></typeparam>
    /// <typeparam name="TMember"></typeparam>
    public interface IPropertyConfigurationExpression<TSource, TDestination, TMember>
    {
        /// <summary>
        /// Ignores a property from being added to the property expression.
        /// </summary>
        public void Ignore();

        /// <summary>
        /// Maps a property from the source type to the destination type using an expression.
        /// </summary>
        /// <param name="source"></param>
        public void MapFrom(Expression<Func<TSource, TMember>> sourceMember);

        /// <summary>
        /// Maps a property from the source type to the destination type using a constant value.
        /// </summary>
        /// <param name="value"></param>
        public void MapFrom(TMember value);
    }

    /// <summary>
    /// <inheritdoc cref="IPropertyConfigurationExpression{TSource, TDestination, TProperty}"/>
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TDestination"></typeparam>
    /// <typeparam name="TMember"></typeparam>
    public class PropertyConfigurationExpression<TSource, TDestination, TMember> : IPropertyConfigurationExpression<TSource, TDestination, TMember>
    {
        private readonly PropertyInfo _propertyInfo;
        private readonly MappingExpression<TSource, TDestination> _mappingExpression;
        private readonly string _propertyPath;
        private static readonly ConcurrentDictionary<(Type, string), Action<object, object?>> _setters = new();

        /// <summary>
        /// Initialize <see cref="PropertyConfigurationExpression{TSource, TDestination, TProperty}"/>.
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <param name="mappingExpression"></param>
        /// <param name="propertyPath"></param>
        public PropertyConfigurationExpression(PropertyInfo propertyInfo, MappingExpression<TSource, TDestination> mappingExpression, string propertyPath)
        {
            _propertyInfo = propertyInfo;
            _mappingExpression = mappingExpression;
            _propertyPath = propertyPath;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void Ignore()
        {
            _mappingExpression.IgnoreProperty(_propertyPath);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="sourceMember"></param>
        public void MapFrom(Expression<Func<TSource, TMember>> sourceMember)
        {
            var function = sourceMember.Compile();
            var setter = GetOrAddSetter(typeof(TDestination), _propertyPath);
            _mappingExpression.AddCustomMap((source, destination) =>
            {
                var sourceValue = function(source);
                var convertedvalue = RooMapper.ConvertValue(sourceValue, _propertyInfo.PropertyType);
                setter(destination!, convertedvalue);
            }, _propertyPath);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="sourceValue"></param>
        public void MapFrom(TMember sourceValue)
        {
            var setter = GetOrAddSetter(typeof(TDestination), _propertyPath);
            _mappingExpression.AddCustomMap((source, destination) =>
            {
                var convertedvalue = RooMapper.ConvertValue(sourceValue, _propertyInfo.PropertyType);
                setter(destination!, convertedvalue);
            }, _propertyPath);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Action<object, object?> GetOrAddSetter(Type type, string propertyPath)
        {
            return _setters.GetOrAdd((type, propertyPath), key =>
            {
                var parameterObject = Expression.Parameter(typeof(object), "obj");
                var parameterValue = Expression.Parameter(typeof(object), "value");
                Expression current = Expression.Convert(parameterObject, type);
                var parts = propertyPath.Split('.');
                for (var i = 0; i < parts.Length - 1; i++)
                {
                    var property = ReflectionCache.GetProperty(type, parts[i]);
                    current = Expression.Property(current, property!);
                    type = property!.PropertyType;
                }
                var finalProperty = ReflectionCache.GetProperty(type, parts[^1])!;
                var assign = Expression.Assign(Expression.Property(current, finalProperty), Expression.Convert(parameterValue, finalProperty.PropertyType));
                var lambda = Expression.Lambda<Action<object, object?>>(assign, parameterObject, parameterValue);
                return lambda.Compile();
            });
        }
    }
}
