using Roo.Azure.Configuration.Common.Utilities.Extensions;
using System.Text;

namespace Roo.Azure.Configuration.Tests.UnitTests
{
    public class ObjectExtensionsTests
    {
        //Common
        private Test test;

        [SetUp]
        public void Setup()
        {
            test = new()
            {
                Id = 1,
                Key = "key",
                Value = "value"
            };
        }

        [Test]
        public void ToQueryString_Verify()
        {
            //Act
            var result = test.ToQueryString();

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result, Is.EqualTo($"?Id={test.Id}&Key={test.Key}&Value={test.Value}"));
            });
        }

        [Test]
        public void ConvertToBase64_Verify()
        {
            //Arrange
            var input = "test";
            var bytes = Encoding.UTF8.GetBytes(input);

            //Act
            var result = new MemoryStream(bytes).ConvertToBase64();

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result, Is.EqualTo(Convert.ToBase64String(Encoding.UTF8.GetBytes(input))));
            });
        }

        private class Test
        {
            public int Id { get; set; }
            public string? Key { get; set; }
            public string? Value { get; set; }
        }
    }
}