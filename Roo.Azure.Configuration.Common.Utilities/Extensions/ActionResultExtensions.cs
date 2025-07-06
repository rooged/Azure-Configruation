using Microsoft.AspNetCore.Mvc;

namespace Roo.Azure.Configuration.Common.Utilities.Extensions
{
    /// <summary>
    /// ActionResult extension methods.
    /// </summary>
    public static class ActionResultExtensions
    {
        /// <summary>
        /// Gets the value of an <see cref="ActionResult{TValue}"/> value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <returns></returns>
        public static T? GetValue<T>(this ActionResult<T> input)
        {
            if (input.Result is OkObjectResult)
            {
                try
                {
                    return (T?)(input.Result as ObjectResult)?.Value;
                }
                catch
                {
                    return default;
                }
            }
            return default;
        }
    }
}
