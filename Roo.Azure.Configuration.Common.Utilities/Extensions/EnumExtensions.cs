using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Roo.Azure.Configuration.Common.Utilities.Extensions
{
    /// <summary>
    /// Enum extension methods.
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// Gets the display name attribute of an <see cref="Enum"/> value. Returns null if name isn't found.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string? GetDisplayName(this Enum input)
        {
            return input.GetType()?.GetMember(input.ToString())?.First()?.GetCustomAttribute<DisplayAttribute>()?.GetName();
        }

        /// <summary>
        /// Gets the null safe display name attribute of an <see cref="Enum"/> value. Returns <see cref="string.Empty"/> if name isn't found.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string GetSafeDisplayName(this Enum input)
        {
            var value = input.GetType()?.GetMember(input.ToString())?.First()?.GetCustomAttribute<DisplayAttribute>()?.GetName();
            return value ?? string.Empty;
        }
    }
}
