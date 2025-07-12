using Roo.Azure.Configuration.Common.Utilities.Extensions;
using System.ComponentModel.DataAnnotations;

namespace Roo.Azure.Configuration.Tests.UnitTests
{
    public class EnumExtensionsTests
    {
        //Common
        private string name = "Name";
        private string name2 = "NameTwo";

        [Test]
        public void GetDisplayName_Verify()
        {
            //Act
            var resultName = Test.Name.GetDisplayName();
            var resultName2 = Test.Name2.GetDisplayName();
            var resultNameNull = Test.NameNull.GetDisplayName();

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(resultName, Is.EqualTo(name));
                Assert.That(resultName2, Is.EqualTo(name2));
                Assert.That(resultNameNull, Is.Null);
            });
        }

        [Test]
        public void GetSafeDisplayName_Verify()
        {
            //Act
            var resultName = Test.Name.GetSafeDisplayName();
            var resultName2 = Test.Name2.GetSafeDisplayName();
            var resultNameNull = Test.NameNull.GetSafeDisplayName();

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(resultName, Is.EqualTo(name));
                Assert.That(resultName2, Is.EqualTo(name2));
                Assert.That(resultNameNull, Is.EqualTo(string.Empty));
            });
        }

        private enum Test
        {
            [Display(Name = "Name")]
            Name = 0,
            [Display(Name = "NameTwo")]
            Name2 = 1,
            NameNull = 3
        }
    }
}