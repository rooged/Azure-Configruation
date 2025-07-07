using Microsoft.AspNetCore.Mvc;
using Roo.Azure.Configuration.Common.Utilities.Extensions;

namespace Roo.Azure.Configuration.UnitTests
{
    public class ActionResultExtensionsTests
    {
        [Test]
        public void GetValue_Verify()
        {
            //Act
            var resultString = TestString().GetValue();
            var resultInt = TestInt().GetValue();
            var resultObject = TestObject().GetValue();

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(resultString, Is.Not.Null);
                Assert.That(resultString, Is.EqualTo("test"));
                Assert.That(resultInt, Is.EqualTo(1));
                Assert.That(resultObject, Is.Not.Null);
                Assert.That(resultObject?.Id, Is.EqualTo(1));
                Assert.That(resultObject?.Key, Is.EqualTo("key"));
                Assert.That(resultObject?.Value, Is.EqualTo("value"));
            });
        }

        private static ActionResult<string> TestString()
        {
            return new OkObjectResult("test");
        }

        private static ActionResult<int> TestInt()
        {
            return new OkObjectResult(1);
        }

        private static ActionResult<Test> TestObject()
        {
            return new OkObjectResult(new Test() { Id = 1, Key = "key", Value = "value" });
        }

        private class Test
        {
            public int Id { get; set; }
            public string? Key { get; set; }
            public string? Value { get; set; }
        }
    }
}