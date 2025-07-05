using System.Text;

namespace Roo.Azure.Configuration.Common.Utilities.Extensions
{
    /// <summary>
    /// Object extension methods.
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// Converts an object to a query string with it's property name-values as the pairs.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ToQueryString(this object input)
        {
            var result = new StringBuilder("?");
            var type = input.GetType();
            var properties = type.GetProperties().Where(x => x.GetValue(input, null) != null).ToList();
            foreach (var property in properties)
            {
                result.Append($"{Uri.EscapeDataString(property.Name)}={Uri.EscapeDataString(property.GetValue(input)?.ToSafeString() ?? "")}&");
            }
            result.Length--;
            return result.ToString();
        }

        /// <summary>
        /// Convert a stream to a string.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static string ConvertToBase64(this Stream stream)
        {
            byte[] bytes;
            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                bytes = memoryStream.ToArray();
            }
            return Convert.ToBase64String(bytes);
        }
    }
}
