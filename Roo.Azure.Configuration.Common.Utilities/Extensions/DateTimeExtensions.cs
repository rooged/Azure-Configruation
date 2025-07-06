namespace Roo.Azure.Configuration.Common.Utilities.Extensions
{
    /// <summary>
    /// DateTime extension methods.
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Adds the number of business days to the current <see cref="DateTime"/>.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="days">Number of business days to be added.</param>
        /// <returns></returns>
        public static DateTime AddBusinessDays(this DateTime input, int days)
        {
            var sign = Math.Sign(days);
            var unsignedDays = Math.Abs(days);
            for (var i = 0; i < unsignedDays; i++)
            {
                do
                {
                    input = input.AddDays(sign);
                }
                while (input.DayOfWeek == DayOfWeek.Saturday || input.DayOfWeek == DayOfWeek.Sunday);
            }
            return input;
        }

        /// <summary>
        /// Gets the number of business days from the date till the to date.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to">DateTime to measure number of business days to.</param>
        /// <returns></returns>
        public static int GetBusinessDaysBetweenDates(this DateTime from, DateTime to)
        {
            var dayDifference = Math.Abs((int)to.Subtract(from).TotalDays);
            return Enumerable.Range(1, dayDifference).Select(x => from.AddDays(x)).Count(x => x.DayOfWeek != DayOfWeek.Saturday && x.DayOfWeek != DayOfWeek.Sunday);
        }

        /// <summary>
        /// Gets the number of days from the date till the to date.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to">DateTime to measure number of days to.</param>
        /// <returns></returns>
        public static int GetDaysBetweenDates(this DateTime from, DateTime to)
        {
            var timespan = to - from;
            return Math.Abs(timespan.Days);
        }

        /// <summary>
        /// Gets the number of years from the date till the to date.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to">DateTime to measure number of years to.</param>
        /// <returns></returns>
        public static int GetYearsBetweenDates(this DateTime from, DateTime to)
        {
            if (to.Month < from.Month || (to.Month == from.Month && to.Day < from.Day))
            {
                return to.Year - from.Year - 1;
            }
            return to.Year - from.Year;
        }

        /// <summary>
        /// Returns a new <see cref="DateTime"/> with the same date and time but without milliseconds.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static DateTime RemoveMilliseconds(this DateTime input)
        {
            return new DateTime(input.Year, input.Month, input.Day, input.Hour, input.Minute, input.Second, input.Kind);
        }

        /// <summary>
        /// Converts the <see cref="DateTime"/> to Eastern Standard Time.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static DateTime ToEst(this DateTime input)
        {
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            return TimeZoneInfo.ConvertTime(input, timeZone);
        }

        /// <summary>
        /// Converts the <see cref="DateTime"/> to Eastern Standard Time.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static DateTime ToEstFromUtc(this DateTime input)
        {
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            return TimeZoneInfo.ConvertTimeFromUtc(input, timeZone);
        }

        /// <summary>
        /// Converts the <see cref="DateTime"/> to Pacific Standard Time.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static DateTime ToPst(this DateTime input)
        {
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
            return TimeZoneInfo.ConvertTime(input, timeZone);
        }

        /// <summary>
        /// Converts the <see cref="DateTime"/> to Pacific Standard Time.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static DateTime ToPstFromUtc(this DateTime input)
        {
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
            return TimeZoneInfo.ConvertTimeFromUtc(input, timeZone);
        }

        /// <summary>
        /// Converts the <see cref="DateTime"/> to Central Standard Time.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static DateTime ToCst(this DateTime input)
        {
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
            return TimeZoneInfo.ConvertTime(input, timeZone);
        }

        /// <summary>
        /// Converts the <see cref="DateTime"/> to Central Standard Time.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static DateTime ToCstFromUtc(this DateTime input)
        {
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
            return TimeZoneInfo.ConvertTimeFromUtc(input, timeZone);
        }
    }
}
