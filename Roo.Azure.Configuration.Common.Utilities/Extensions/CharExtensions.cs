namespace Roo.Azure.Configuration.Common.Utilities.Extensions
{
    /// <summary>
    /// String extension methods.
    /// </summary>
    public static class CharExtensions
    {
        private static readonly Dictionary<char, char> upperToLower = new() { { 'A', 'a' }, { 'B', 'b' }, { 'C', 'c' }, { 'D', 'd' }, { 'E', 'e' }, { 'F', 'f' }, { 'G', 'g' }, { 'H', 'h' }, { 'I', 'i' }, { 'J', 'j' }, { 'K', 'k' },
            { 'L', 'l' }, { 'M', 'm' }, { 'N', 'n' }, { 'O', 'o' }, { 'P', 'p' }, { 'Q', 'q' }, { 'R', 'r' }, { 'S', 's' }, { 'T', 't' }, { 'U', 'u' }, { 'V', 'v' }, { 'W', 'w' },{ 'X', 'x' }, { 'Y', 'y' }, { 'Z', 'z' } };
        private static readonly Dictionary<char, char> lowerToUpper = new() { { 'a', 'A' }, { 'b', 'B' }, { 'c', 'C' }, { 'd', 'D' }, { 'e', 'D' }, { 'f', 'F' }, { 'g', 'G' }, { 'h', 'H' }, { 'i', 'I' }, { 'j', 'J' }, { 'k', 'K' },
            { 'l', 'L' }, { 'm', 'M' }, { 'n', 'N' }, { 'o', 'O' }, { 'p', 'P' }, { 'q', 'Q' }, { 'r', 'R' }, { 's', 'S' }, { 't', 'T' }, { 'u', 'U' }, { 'v', 'V' }, { 'w', 'W' },{ 'x', 'X' }, { 'y', 'Y' }, { 'z', 'Z' } };
        private static readonly HashSet<char> letters = [ 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f', 'g',
            'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' ];
        private static readonly HashSet<char> numbers = ['0', '1', '2', '3', '4', '5', '6', '7', '8', '9'];

        /// <summary>
        /// Converts a lowercase letter to an uppercase letter. Returns the original character if not an alphabetical letter.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static char ToLower(this char value)
        {
            if (!upperToLower.TryGetValue(value, out var upperValue))
            {
                return value;
            }
            return upperValue;
        }

        /// <summary>
        /// Converts an uppercase letter to a lowercase letter. Returns the original character if not an alphabetical letter.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static char ToUpper(this char value)
        {
            if (!lowerToUpper.TryGetValue(value, out var lowerValue))
            {
                return value;
            }
            return lowerValue;
        }

        /// <summary>
        /// Check if a character is a letter (a - z, A - Z). Return true if so, false if not.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsLetter(this char value)
        {
            return letters.TryGetValue(value, out _);
        }

        /// <summary>
        /// Check if a character is an uppercase letter (A - Z). Return true if so, false if not.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsUpper(this char value)
        {
            return upperToLower.TryGetValue(value, out _);
        }

        /// <summary>
        /// Check if a character is a lowercase letter (a - z). Return true if so, false if not.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsLower(this char value)
        {
            return lowerToUpper.TryGetValue(value, out _);
        }

        /// <summary>
        /// Check if a character is a digit (0 - 9). Return true if so, false if not.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsDigit(this char value)
        {
            return numbers.TryGetValue(value, out _);
        }

        /// <summary>
        /// Check if a character is a letter (a - z, A - Z) or digit (0 - 9). Return true if so, false if not.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsAlphanumeric(this char value)
        {
            return letters.TryGetValue(value, out _) || numbers.TryGetValue(value, out _);
        }
    }
}
