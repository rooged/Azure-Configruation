using Microsoft.FeatureManagement;

namespace Roo.Azure.Configuration.UnitTests
{
    public class FeatureManagerServiceTests
    {
        //Common
        private readonly string featureName = "featureName";

        [Test]
        public async Task IsFeatureEnabled_Verify()
        {
            //Arrange
            var featureManagerService = new Mock<IFeatureManager>();
            featureManagerService.Setup(x => x.IsEnabledAsync(It.IsAny<string>())).Returns(Task.FromResult(true));
            var service = new FeatureManagerService(featureManagerService.Object);

            //Act
            var result = await service.IsFeatureEnabled(featureName);

            //Assert
            using (Assert.EnterMultipleScope())
            {
                featureManagerService.Verify(x => x.IsEnabledAsync(It.IsAny<string>()), Times.Once);
                Assert.That(result, Is.True);
            }
        }

        [Test]
        public async Task IsFeatureEnabledType_Verify()
        {
            //Arrange
            var featureManagerService = new Mock<IFeatureManager>();
            featureManagerService.Setup(x => x.IsEnabledAsync(It.IsAny<string>(), It.IsAny<It.IsAnyType>())).Returns(Task.FromResult(true));
            var service = new FeatureManagerService(featureManagerService.Object);

            //Act
            var result = await service.IsFeatureEnabled(featureName, "context");

            //Assert
            using (Assert.EnterMultipleScope())
            {
                featureManagerService.Verify(x => x.IsEnabledAsync(It.IsAny<string>(), It.IsAny<It.IsAnyType>()), Times.Once);
                Assert.That(result, Is.True);
            }
        }

        [Test]
        public async Task GetFeatureNames_Verify()
        {
            //Arrange
            var asyncReturn = ToAsyncEnumerable(new List<string> { featureName });
            var featureManagerService = new Mock<IFeatureManager>();
            featureManagerService.Setup(x => x.GetFeatureNamesAsync()).Returns(asyncReturn);
            var service = new FeatureManagerService(featureManagerService.Object);

            //Act
            var asyncResults = service.GetFeatureNames();
            string? result = null;
            await foreach (var asyncResult in asyncResults)
            {
                result = asyncResult;
            }

            //Assert
            using (Assert.EnterMultipleScope())
            {
                featureManagerService.Verify(x => x.GetFeatureNamesAsync(), Times.Once);
                Assert.That(result, Is.EqualTo(featureName));
            }
        }

        private static async IAsyncEnumerable<T> ToAsyncEnumerable<T>(IEnumerable<T> values)
        {
            foreach (var value in values)
            {
                yield return value;
            }
            await Task.Delay(1);
        }
    }
}