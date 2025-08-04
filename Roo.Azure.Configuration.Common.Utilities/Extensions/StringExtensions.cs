using System.Globalization;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Roo.Azure.Configuration.Common.Utilities.Extensions
{
    /// <summary>
    /// String extension methods.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Converts a string to a nullable boolean. Returns null if value can't be converted.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool? ToBool(this string? input)
        {
            if (bool.TryParse(input, out var result))
            {
                return result;
            }
            return null;
        }

        /// <summary>
        /// Converts a string to a null safe boolean. Returns false if value can't be converted.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool ToSafeBool(this string? input)
        {
            if (bool.TryParse(input, out var result))
            {
                return result;
            }
            return false;
        }

        /// <summary>
        /// Converts a string a nullable decimal. Returns null if value can't be converted.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static decimal? ToDecimal(this string? input)
        {
            if (decimal.TryParse(input, out var result))
            {
                return result;
            }
            return null;
        }

        /// <summary>
        /// Converts a string a safe null decimal. Returns 0 if value can't be converted.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static decimal ToSafeDecimal(this string? input)
        {
            if (decimal.TryParse(input, out var result))
            {
                return result;
            }
            return 0;
        }

        /// <summary>
        /// Converts a string a nullable double. Returns null if value can't be converted.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static double? ToDouble(this string? input)
        {
            if (double.TryParse(input, out var result))
            {
                return result;
            }
            return null;
        }

        /// <summary>
        /// Converts a string a safe null double. Returns 0 if value can't be converted.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static double ToSafeDouble(this string? input)
        {
            if (double.TryParse(input, out var result))
            {
                return result;
            }
            return 0;
        }

        /// <summary>
        /// Converts a string to a nullable integer. Returns null if value can't be converted.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static int? ToInt(this string? input)
        {
            if (int.TryParse(input, out var result))
            {
                return result;
            }
            return null;
        }

        /// <summary>
        /// Converts a string to a null safe integer. Returns 0 if value can't be converted.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static int ToSafeInt(this string? input)
        {
            if (int.TryParse(input, out var result))
            {
                return result;
            }
            return 0;
        }

        /// <summary>
        /// Converts a string to a nullable long. Returns null if value can't be converted.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static long? ToLong(this string? input)
        {
            if (long.TryParse(input, out var result))
            {
                return result;
            }
            return null;
        }

        /// <summary>
        /// Converts a string to a null safe long. Returns 0 if value can't be converted.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static long ToSafeLong(this string? input)
        {
            if (long.TryParse(input, out var result))
            {
                return result;
            }
            return 0;
        }

        /// <summary>
        /// Converts a string to a nullable float. Returns null if value can't be converted.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static float? ToFloat(this string? input)
        {
            if (float.TryParse(input, out var result))
            {
                return result;
            }
            return null;
        }

        /// <summary>
        /// Converts a string to a null safe float. Returns 0 if value can't be converted.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static float ToSafeFloat(this string? input)
        {
            if (float.TryParse(input, out var result))
            {
                return result;
            }
            return 0;
        }

        /// <summary>
        /// Converts a string to a nullable short. Returns null if value can't be converted.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static short? ToShort(this string? input)
        {
            if (short.TryParse(input, out var result))
            {
                return result;
            }
            return null;
        }

        /// <summary>
        /// Converts a string to a null safe short. Returns 0 if value can't be converted.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static short ToSafeShort(this string? input)
        {
            if (short.TryParse(input, out var result))
            {
                return result;
            }
            return 0;
        }

        /// <summary>
        /// Converts a string to a nullable DateTime. Returns null if value can't be converted.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static DateTime? ToDateTime(this string? input)
        {
            if (DateTime.TryParse(input, out var result))
            {
                return result;
            }
            return null;
        }

        /// <summary>
        /// Converts a string to a null safe DateTime. Returns DateTime.MinValue if value can't be converted.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static DateTime ToSafeDateTime(this string? input)
        {
            if (DateTime.TryParse(input, out var result))
            {
                return result;
            }
            return DateTime.MinValue;
        }

        /// <summary>
        /// Converts a string to a nullable Guid. Returns null if value can't be converted.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static Guid? ToGuid(this string? input)
        {
            if (Guid.TryParse(input, out var result))
            {
                return result;
            }
            return null;
        }

        /// <summary>
        /// Converts a string to a null safe Guid. Returns Guid.Empty (all 0's) if value can't be converted.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static Guid ToSafeGuid(this string? input)
        {
            if (Guid.TryParse(input, out var result))
            {
                return result;
            }
            return Guid.Empty;
        }

        /// <summary>
        /// Converts the first letter of each word to upper case and the rest lower case. Option to include or ignore words that are all capital letters. Returns <see cref="string.Empty"/> if value is null or empty.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="includeAllCapitalWords">Whether to convert words that are all capital letters to title case as well, default is true.</param>
        /// <returns></returns>
        public static string ToTitleCase(this string? input, bool includeAllCapitalWords = true)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }
            if (includeAllCapitalWords)
            {
                return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(input.ToLower());
            }
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(input);
        }

        /// <summary>
        /// Returns only the numbers from a string. Returns <see cref="string.Empty"/> if value is null or empty.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ToNumbersOnly(this string? input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }
            var value = new StringBuilder();
            for (var i = 0; i < input.Length; i++)
            {
                if (input[i].IsDigit())
                {
                    value.Append(input[i]);
                }
            }
            return value.ToString();
        }

        /// <summary>
        /// Safe Remove so exception won't be thrown when removing extra characters from a string. Returns original string if value can't be truncated. Returns <see cref="string.Empty"/> if value is null or empty.
        /// </summary>
        /// <param name="input">String value to be trimmed.</param>
        /// <param name="maxLength">Maximum length of string to trim to.</param>
        /// <returns></returns>
        public static string Truncate(this string? input, int maxLength)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }
            if (input.Length > maxLength)
            {
                return input.Remove(maxLength);
            }
            return input;
        }

        /// <summary>
        /// Checks if a string is in a valid email address format. Returns false if value is null or empty.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsValidEmailFormat(this string? input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return false;
            }
            try
            {
                var mailAddress = new MailAddress(input);
                return mailAddress.Address.Equals(input);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Converts a string to an escaped safe string. Returns <see cref="string.Empty"/> if the object is null or if the string conversion of it results in a null or empty string.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ToSafeString(this object? input)
        {
            if (input == null)
            {
                return string.Empty;
            }
            var value = input.ToString();
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }
            var pattern = @"\s+";
            return HttpUtility.HtmlEncode(Regex.Replace(value, pattern, " "));
        }

        /// <summary>
        /// Ensure a string ends with another string, adds it if not. Returns <see cref="string.Empty"/> if input is null or empty.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string EnsureEndsWith(this string? input, string endsWith)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }
            if (!input.EndsWith(endsWith))
            {
                return input + endsWith;
            }
            return input;
        }

        /// <summary>
        /// Masks the beginning of an email but leaves domain untouched. Times out if it takes longer than 90 seconds. Returns <see cref="string.Empty"/> if input is null or empty.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string MaskEmail(this string? input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }
            var pattern = "(?<=^[\\w\\-._\\+%]{2})[^@]*@";
            var value = Regex.Replace(input, pattern, x => "***@", new RegexOptions(), TimeSpan.FromSeconds(90));
            return value;
        }

        /// <summary>
        /// Converts a the first character of a string to an uppercase letter. Returns <see cref="string.Empty"/> if input is null or empty.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string FirstCharToUpper(this string? input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }
            if (input.First().IsUpper())
            {
                return input;
            }
            var value = new StringBuilder();
            value.Append(input.First().ToUpper());
            value.Append(input.AsSpan(1));
            return value.ToString();
        }

        /// <summary>
        /// Converts a string to a base 64 encoded.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string Base64Encode(this string input)
        {
            var bytes = Encoding.UTF8.GetBytes(input);
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// <inheritdoc cref="string.IsNullOrEmpty(string?)"/> Does the same as string.IsNullOrEmpty() just as a value extension.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this string? input)
        {
            if (input ==  null || input.Length == 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// <inheritdoc cref="string.IsNullOrWhiteSpace(string?)"/> Does the same as string.IsNullOrWhiteSpace() just as a value extension.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsNullEmptyOrWhitespace(this string? input)
        {
            if (input == null || input.Length == 0)
            {
                return true;
            }
            for (var i = 0; i < input.Length - 1; i++)
            {
                if (!char.IsWhiteSpace(input[i]))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
