using Roo.Azure.Configuration.Common.Utilities.Extensions;
using System.ComponentModel.DataAnnotations;

namespace Roo.Azure.Configuration.Tests.UnitTests
{
    public class DateTimeExtensionsTests
    {
        [Test]
        public void AddBusinessDays_Verify()
        {
            //Arrange
            var date = DateTime.Now;
            while (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
            {
                date = date.AddDays(-1);
            }
            var dateSaturday = date;
            while (dateSaturday.DayOfWeek != DayOfWeek.Saturday)
            {
                dateSaturday = dateSaturday.AddDays(1);
            }
            var dateSunday = dateSaturday.AddDays(1);

            //Act
            var result = date.AddBusinessDays(5);
            var resultSaturday = dateSaturday.AddBusinessDays(5);
            var resultSunday = dateSunday.AddBusinessDays(7);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.EqualTo(date.AddDays(7)));
                Assert.That(resultSaturday, Is.EqualTo(dateSaturday.AddDays(6)));
                Assert.That(resultSunday, Is.EqualTo(dateSunday.AddDays(9)));
            });
        }

        [Test]
        public void GetBusinessDaysBetweenDates_Verify()
        {
            //Arrange
            var date = DateTime.Now;
            while (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
            {
                date = date.AddDays(-1);
            }
            var dateSaturday = date;
            while (dateSaturday.DayOfWeek != DayOfWeek.Saturday)
            {
                dateSaturday = dateSaturday.AddDays(1);
            }
            var dateSunday = dateSaturday.AddDays(1);

            //Act
            var result = date.GetBusinessDaysBetweenDates(date.AddDays(7));
            var resultSaturday = dateSaturday.GetBusinessDaysBetweenDates(dateSaturday.AddDays(7));
            var resultSunday = dateSunday.GetBusinessDaysBetweenDates(dateSunday.AddDays(9));

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.EqualTo(5));
                Assert.That(resultSaturday, Is.EqualTo(5));
                Assert.That(resultSunday, Is.EqualTo(7));
            });
        }

        [Test]
        public void GetDaysBetweenDates_Verify()
        {
            //Arrange
            var date = DateTime.Now;

            //Act
            var result = date.GetDaysBetweenDates(date.AddDays(7));
            var resultYear = date.GetDaysBetweenDates(date.AddYears(4));

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.EqualTo(7));
                Assert.That(resultYear, Is.EqualTo(1461));
            });
        }

        [Test]
        public void GetYearsBetweenDates_Verify()
        {
            //Arrange
            var date = DateTime.Now;

            //Act
            var result = date.GetYearsBetweenDates(date.AddMonths(7));
            var resultYear = date.GetYearsBetweenDates(date.AddYears(4));
            var resultYearLessThan = date.GetYearsBetweenDates(date.AddMonths(23));

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.EqualTo(0));
                Assert.That(resultYear, Is.EqualTo(4));
                Assert.That(resultYearLessThan, Is.EqualTo(1));
            });
        }

        [Test]
        public void RemoveMilliseconds_Verify()
        {
            //Arrange
            var date = DateTime.Now;

            //Act
            var result = date.RemoveMilliseconds();

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.ToString(), Is.EqualTo(date.ToString().TrimEnd('.')));
            });
        }

        [Test]
        public void ToEst_Verify()
        {
            //Arrange
            var date = DateTime.Now;

            //Act
            var result = date.ToEst();

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.EqualTo(TimeZoneInfo.ConvertTime(date, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"))));
            });
        }

        [Test]
        public void ToEstFromUtc_Verify()
        {
            //Arrange
            var date = DateTime.UtcNow;

            //Act
            var result = date.ToEst();

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.EqualTo(TimeZoneInfo.ConvertTimeFromUtc(date, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"))));
            });
        }

        [Test]
        public void ToPst_Verify()
        {
            //Arrange
            var date = DateTime.Now;

            //Act
            var result = date.ToPst();

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.EqualTo(TimeZoneInfo.ConvertTime(date, TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time"))));
            });
        }

        [Test]
        public void ToPstFromUtc_Verify()
        {
            //Arrange
            var date = DateTime.UtcNow;

            //Act
            var result = date.ToPst();

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.EqualTo(TimeZoneInfo.ConvertTimeFromUtc(date, TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time"))));
            });
        }

        [Test]
        public void ToCst_Verify()
        {
            //Arrange
            var date = DateTime.Now;

            //Act
            var result = date.ToCst();

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.EqualTo(TimeZoneInfo.ConvertTime(date, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))));
            });
        }

        [Test]
        public void ToCstFromUtc_Verify()
        {
            //Arrange
            var date = DateTime.UtcNow;

            //Act
            var result = date.ToCst();

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.EqualTo(TimeZoneInfo.ConvertTimeFromUtc(date, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))));
            });
        }
    }
}