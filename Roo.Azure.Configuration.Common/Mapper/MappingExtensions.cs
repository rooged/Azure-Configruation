using System.Collections;

namespace Roo.Azure.Configuration.Common.Mapper
{
    internal class MappingExtensions
    {
        internal static bool IsComplexType(Type type)
        {
            return type.IsClass && type != typeof(string);
        }

        internal static bool IsCollectionType(Type type, out Type? elementType)
        {
            if (type == typeof(string))
            {
                elementType = null;
                return false;
            }
            if (type.IsArray)
            {
                elementType = type.GetElementType();
                return true;
            }
            if (type.GetInterface(nameof(IEnumerable)) != null)
            {
                var enumerableInterface = type.GetInterfaces().FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEnumerable<>));
                if (enumerableInterface != null)
                {
                    elementType = enumerableInterface.GetGenericArguments()[0];
                    return true;
                }
            }
            elementType = null;
            return false;
        }

        internal static bool IsEnumerableType(Type type, out Type? elementType)
        {
            if (type == typeof(string))
            {
                elementType = null;
                return false;
            }
            if (type.IsArray)
            {
                elementType = type.GetElementType();
                return true;
            }
            if (type.IsGenericType && typeof(IEnumerable<>).IsAssignableFrom(type.GetGenericTypeDefinition()))
            {
                elementType = type.GetGenericArguments()[0];
                return true;
            }
            var interfaceType = type.GetInterfaces().FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEnumerable<>));
            if (interfaceType != null)
            {
                elementType = interfaceType.GetGenericArguments()[0];
                return true;
            }
            elementType = null;
            return false;
        }
    }
}
