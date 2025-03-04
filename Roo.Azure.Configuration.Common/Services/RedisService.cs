using Newtonsoft.Json;
using StackExchange.Redis;

namespace Roo.Azure.Configuration.Common.Services
{
    /// <summary>
    /// Service extension for accessing Redis methods.
    /// </summary>
    public interface IRedisService
    {
        /// <summary>
        /// Get cached string based on key.
        /// </summary>
        /// <param name="key">Key of value stored in cache.</param>
        /// <returns>String from cache.</returns>
        Task<string?> Get(string key);
        /// <summary>
        /// Get cached string with expiration time in Epoch based on key.
        /// </summary>
        /// <param name="key">Key of value stored in cache.</param>
        /// <returns>Cached string with expiration time in Epoch from cache.</returns>
        Task<(string? token, TimeSpan? expirationTimeSpan)> GetWithExpiration(string key);
        /// <summary>
        /// Get cached object of type T based on key.
        /// </summary>
        /// <typeparam name="T">Object type to be returned.</typeparam>
        /// <param name="key">Key of object stored in cache.</param>
        /// <returns>Object from cache.</returns>
        Task<T?> GetObject<T>(string key);
        /// <summary>
        /// Get field from cached object based on key.
        /// </summary>
        /// <param name="key">Key of object stored in cache.</param>
        /// <param name="fieldName">Name of field in obejct stored in cache.</param>
        /// <returns>Field in object from cache.</returns>
        Task<string?> GetField(string key, string field);
        /// <summary>
        /// Set string in cache.
        /// </summary>
        /// <param name="key">Key to store value under in cache.</param>
        /// <param name="value">Value to store in cache.</param>
        /// <returns>Whether value was cached.</returns>
        Task<bool> Set(string key, string value);
        /// <summary>
        /// Set string with expiration in cache.
        /// </summary>
        /// <param name="key">Key to store value under in cache.</param>
        /// <param name="value">Value to store in cache.</param>
        /// <param name="expiration">Expiration time of value.</param>
        /// <returns>Whether value was cached.</returns>
        Task<bool> Set(string key, string value, TimeSpan expiration);
        /// <summary>
        /// Set object in cache, overwrites the object if key already exists in cache.
        /// </summary>
        /// <param name="key">Key to store value under in cache.</param>
        /// <param name="value">Value to store in cache.</param>
        /// <returns>Whether value was cached.</returns>
        Task SetObject<T>(string key, T value);
        /// <summary>
        /// Set field in object in cache.
        /// </summary>
        /// <param name="key">Key to store value under in cache.</param>
        /// <param name="field">Field to store value in in cache.</param>
        /// <param name="value">Value to store in cache.</param>
        /// <returns>Whether value was cached.</returns>
        Task<bool> SetField(string key, string field, string value);
        /// <summary>
        /// Delete item from cache.
        /// </summary>
        /// <param name="key">Key of item to delete from cache.</param>
        /// <returns>Whether item was deleted from cache.</returns>
        Task<bool> Delete(string key);
        /// <summary>
        /// Delete field from item from cache.
        /// </summary>
        /// <param name="key">Key of item to delete field from cache.</param>
        /// <param name="field">Field to delete from cache.</param>
        /// <returns>Whether field in item was deleted from cache.</returns>
        Task<bool> DeleteField(string key, string field);
        /// <summary>
        /// Convert an object to HashEntry.
        /// </summary>
        /// <param name="obj">Object to convert.</param>
        /// <returns>HashEntry of object.</returns>
        HashEntry[] ObjectToHashEntry(object obj);
        /// <summary>
        /// Convert a HashEntry to an object.
        /// </summary>
        /// <typeparam name="T">Type of object to convert to.</typeparam>
        /// <param name="hashEntry">HashEntry to convert.</param>
        /// <returns>Object from HashEntry.</returns>
        T HashEntryToObject<T>(HashEntry[] hashEntry);
    }

    /// <summary>
    /// Implementation of <see cref="IRedisService"/>
    /// </summary>
    public class RedisService : IRedisService
    {
        private readonly IConnectionMultiplexer _redis;

        /// <summary>
        /// Initialize <see cref="RedisService"/>
        /// </summary>
        /// <param name="redis"></param>
        public RedisService(IConnectionMultiplexer redis)
        {
            _redis = redis;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<string?> Get(string key)
        {
            return await _redis.GetDatabase().StringGetAsync(key);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<(string? token, TimeSpan? expirationTimeSpan)> GetWithExpiration(string key)
        {
            var result = await _redis.GetDatabase().StringGetWithExpiryAsync(key);
            return (result.Value, result.Expiry);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<T?> GetObject<T>(string key)
        {
            var result = await _redis.GetDatabase().HashGetAllAsync(key);
            return HashEntryToObject<T>(result);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public async Task<string?> GetField(string key, string field)
        {
            return await _redis.GetDatabase().HashGetAsync(key, field);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task<bool> Set(string key, string value)
        {
            return await _redis.GetDatabase().StringSetAsync(key, value, null);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiration"></param>
        /// <returns></returns>
        public async Task<bool> Set(string key, string value, TimeSpan expiration)
        {
            return await _redis.GetDatabase().StringSetAsync(key, value, expiration);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task SetObject<T>(string key, T value)
        {
            if (value == null)
            {
                return;
            }
            var result = ObjectToHashEntry(value);
            await _redis.GetDatabase().HashSetAsync(key, result);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task<bool> SetField(string key, string field, string value)
        {
            return await _redis.GetDatabase().HashSetAsync(key, field, value);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<bool> Delete(string key)
        {
            return await _redis.GetDatabase().KeyDeleteAsync(key);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public async Task<bool> DeleteField(string key, string field)
        {
            return await _redis.GetDatabase().HashDeleteAsync(key, field);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public HashEntry[] ObjectToHashEntry(object obj)
        {
            var properties = obj.GetType().GetProperties();
            return properties.Where(x => x.GetValue(obj) != null).Select(property =>
            {
                var propertyValue = property.GetValue(obj);
                var hashValue = "";

                if (propertyValue is IEnumerable<object>)
                {
                    hashValue = JsonConvert.SerializeObject(propertyValue);
                }
                else
                {
                    hashValue = propertyValue?.ToString();
                }

                return new HashEntry(property.Name, hashValue);
            }).ToArray();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="hashEntry"></param>
        /// <returns></returns>
        public T HashEntryToObject<T>(HashEntry[] hashEntry)
        {
            var properties = typeof(T).GetProperties();
            var obj = Activator.CreateInstance(typeof(T)) ?? new();
            foreach (var property in properties)
            {
                var entry = hashEntry.FirstOrDefault(x => x.Name.ToString().Equals(property.Name));
                if (entry.Equals(new HashEntry()))
                {
                    continue;
                }
                property.SetValue(obj, Convert.ChangeType(entry.Value.ToString(), property.PropertyType));
            }
            return (T)obj;
        }
    }
}
