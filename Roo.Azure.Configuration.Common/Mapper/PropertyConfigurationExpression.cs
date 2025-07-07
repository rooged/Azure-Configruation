using System.Linq.Expressions;
using System.Reflection;

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
            _mappingExpression.AddCustomMap((source, destination) =>
            {
                var sourceValue = function(source);
                var convertedvalue = RooMapper.ConvertValue(sourceValue, _propertyInfo.PropertyType);
                SetNestedPropertyValue(destination, _propertyPath, convertedvalue);
            }, _propertyPath);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="sourceValue"></param>
        public void MapFrom(TMember sourceValue)
        {
            _mappingExpression.AddCustomMap((source, destination) =>
            {
                var convertedvalue = RooMapper.ConvertValue(sourceValue, _propertyInfo.PropertyType);
                SetNestedPropertyValue(destination, _propertyPath, convertedvalue);
            }, _propertyPath);
        }

        private static void SetNestedPropertyValue(object? destination, string propertyPath, object? value)
        {
            var pathParts = propertyPath.Split('.');
            object? current = destination;
            PropertyInfo? currentProperty = null;
            for (int i = 0; i < pathParts.Length - 1; i++)
            {
                currentProperty = current!.GetType().GetProperty(pathParts[i]);
                if (currentProperty == null)
                {
                    throw new InvalidOperationException($"Property '{pathParts[i]}' not found on type '{current!.GetType().FullName}'.");
                }
                var next = currentProperty.GetValue(current);
                if (next == null)
                {
                    next = Activator.CreateInstance(currentProperty.PropertyType)!;
                    currentProperty.SetValue(current, next);
                }
                current = next;
            }

            var finalProperty = current!.GetType().GetProperty(pathParts[^1]);
            if (finalProperty == null)
            {
                throw new InvalidOperationException($"Property '{pathParts[^1]}' not found on type '{current!.GetType().FullName}'.");
            }
            finalProperty.SetValue(current, value);
        }
    }
}
