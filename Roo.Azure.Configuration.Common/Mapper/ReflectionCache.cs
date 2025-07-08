using System.Collections.Concurrent;
using System.Reflection;

namespace Roo.Azure.Configuration.Common.Mapper
{
    internal static class ReflectionCache
    {
        private static readonly ConcurrentDictionary<(Type, string), PropertyInfo?> _propertyCache = new();

        public static PropertyInfo? GetProperty(Type type, string propertyName)
        {
            return _propertyCache.GetOrAdd((type, propertyName), key =>
            {
                var (keyType, keyName) = key;
                return keyType.GetProperty(keyName, BindingFlags.Public | BindingFlags.Instance);
            });
        }
    }
}
