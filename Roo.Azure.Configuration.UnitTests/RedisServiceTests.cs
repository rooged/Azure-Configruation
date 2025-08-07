using StackExchange.Redis;

namespace Roo.Azure.Configuration.UnitTests
{
    public class RedisServiceTests
    {
        //Common
        private Mock<IConnectionMultiplexer> connectionMultiplexerMoq;
        private RedisService serviceConversion;
        private readonly string key = "key";
        private readonly string value = "value";
        private Test test;
        private readonly TimeSpan expirationTime = new(0, 5, 0);

        [SetUp]
        public void Setup()
        {
            connectionMultiplexerMoq = new();
            serviceConversion = new(connectionMultiplexerMoq.Object);
            test = new()
            {
                Key = key,
                Value = value
            };
        }

        [Test]
        public async Task GetString_Verify()
        {
            //Arrange
            var repository = new Mock<IConnectionMultiplexer>();
            repository.Setup(x => x.GetDatabase(It.IsAny<int>(), It.IsAny<object>()).StringGetAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>())).Returns(Task.FromResult(new RedisValue(value)));
            var service = new RedisService(repository.Object);

            //Act
            var result = await service.Get(key);

            //Assert
            using (Assert.EnterMultipleScope())
            {
                repository.Verify(x => x.GetDatabase(It.IsAny<int>(), It.IsAny<object>()).StringGetAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()), Times.Once);
                Assert.That(result, Is.EqualTo(value));
            }
        }

        [Test]
        public async Task GetStringWithExpiration_Verify()
        {
            //Arrange
            var repository = new Mock<IConnectionMultiplexer>();
            repository.Setup(x => x.GetDatabase(It.IsAny<int>(), It.IsAny<object>()).StringGetWithExpiryAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>())).Returns(Task.FromResult(new RedisValueWithExpiry(value, expirationTime)));
            var service = new RedisService(repository.Object);

            //Act
            var (token, expirationTimeSpan) = await service.GetWithExpiration(key);

            //Assert
            using (Assert.EnterMultipleScope())
            {
                repository.Verify(x => x.GetDatabase(It.IsAny<int>(), It.IsAny<object>()).StringGetWithExpiryAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()), Times.Once);
                Assert.That(token, Is.EqualTo(value));
                Assert.That(expirationTimeSpan, Is.EqualTo(expirationTime));
            }
        }

        [Test]
        public async Task GetObject_Verify()
        {
            //Arrange
            var hashedTest = serviceConversion.ObjectToHashEntry(test);
            var repository = new Mock<IConnectionMultiplexer>();
            repository.Setup(x => x.GetDatabase(It.IsAny<int>(), It.IsAny<object>()).HashGetAllAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>())).Returns(Task.FromResult(hashedTest));
            var service = new RedisService(repository.Object);

            //Act
            var result = await service.GetObject<Test>(key);

            //Assert
            using (Assert.EnterMultipleScope())
            {
                repository.Verify(x => x.GetDatabase(It.IsAny<int>(), It.IsAny<object>()).HashGetAllAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()), Times.Once);
                Assert.That(result, Is.Not.Null);
                Assert.That(result?.Key, Is.EqualTo(key));
                Assert.That(result?.Value, Is.EqualTo(value));
            }
        }

        [Test]
        public async Task GetField_Verify()
        {
            //Arrange
            var repository = new Mock<IConnectionMultiplexer>();
            repository.Setup(x => x.GetDatabase(It.IsAny<int>(), It.IsAny<object>()).HashGetAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<CommandFlags>())).Returns(Task.FromResult(new RedisValue(value)));
            var service = new RedisService(repository.Object);

            //Act
            var result = await service.GetField(key, value);

            //Assert
            using (Assert.EnterMultipleScope())
            {
                repository.Verify(x => x.GetDatabase(It.IsAny<int>(), It.IsAny<object>()).HashGetAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<CommandFlags>()), Times.Once);
                Assert.That(result, Is.Not.Null);
                Assert.That(result, Is.EqualTo(value));
            }
        }

        [Test]
        public async Task SetString_Verify()
        {
            //Arrange
            var repository = new Mock<IConnectionMultiplexer>();
            repository.Setup(x => x.GetDatabase(It.IsAny<int>(), It.IsAny<object>()).StringSetAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<TimeSpan?>(), It.IsAny<bool>(), It.IsAny<When>(), It.IsAny<CommandFlags>())).Returns(Task.FromResult(true));
            var service = new RedisService(repository.Object);

            //Act
            var result = await service.Set(key, value);

            //Assert
            using (Assert.EnterMultipleScope())
            {
                repository.Verify(x => x.GetDatabase(It.IsAny<int>(), It.IsAny<object>()).StringSetAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), null, It.IsAny<bool>(), It.IsAny<When>(), It.IsAny<CommandFlags>()), Times.Once);
                Assert.That(result, Is.True);
            }
        }

        [Test]
        public async Task SetStringWithExpiration_Verify()
        {
            //Arrange
            var repository = new Mock<IConnectionMultiplexer>();
            repository.Setup(x => x.GetDatabase(It.IsAny<int>(), It.IsAny<object>()).StringSetAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<TimeSpan?>(), It.IsAny<bool>(), It.IsAny<When>(), It.IsAny<CommandFlags>())).Returns(Task.FromResult(true));
            var service = new RedisService(repository.Object);

            //Act
            var result = await service.Set(key, value, expirationTime);

            //Assert
            using (Assert.EnterMultipleScope())
            {
                repository.Verify(x => x.GetDatabase(It.IsAny<int>(), It.IsAny<object>()).StringSetAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<TimeSpan?>(), It.IsAny<bool>(), It.IsAny<When>(), It.IsAny<CommandFlags>()), Times.Once);
                Assert.That(result, Is.True);
            }
        }

        [Test]
        public async Task SetObject_Verify()
        {
            //Arrange
            var repository = new Mock<IConnectionMultiplexer>();
            repository.Setup(x => x.GetDatabase(It.IsAny<int>(), It.IsAny<object>()).HashSetAsync(It.IsAny<RedisKey>(), It.IsAny<HashEntry[]>(), It.IsAny<CommandFlags>()));
            var service = new RedisService(repository.Object);

            //Act
            await service.SetObject(key, test);

            //Assert
            repository.Verify(x => x.GetDatabase(It.IsAny<int>(), It.IsAny<object>()).HashSetAsync(It.IsAny<RedisKey>(), It.IsAny<HashEntry[]>(), It.IsAny<CommandFlags>()), Times.Once);
        }

        [Test]
        public async Task SetField_Verify()
        {
            //Arrange
            var repository = new Mock<IConnectionMultiplexer>();
            repository.Setup(x => x.GetDatabase(It.IsAny<int>(), It.IsAny<object>()).HashSetAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<RedisValue>(), It.IsAny<When>(), It.IsAny<CommandFlags>())).Returns(Task.FromResult(true));
            var service = new RedisService(repository.Object);

            //Act
            var result = await service.SetField(key, value, value);

            //Assert
            using (Assert.EnterMultipleScope())
            {
                repository.Verify(x => x.GetDatabase(It.IsAny<int>(), It.IsAny<object>()).HashSetAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<RedisValue>(), It.IsAny<When>(), It.IsAny<CommandFlags>()), Times.Once);
                Assert.That(result, Is.True);
            }
        }

        [Test]
        public async Task Delete_Verify()
        {
            //Arrange
            var repository = new Mock<IConnectionMultiplexer>();
            repository.Setup(x => x.GetDatabase(It.IsAny<int>(), It.IsAny<object>()).KeyDeleteAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>())).Returns(Task.FromResult(true));
            var service = new RedisService(repository.Object);

            //Act
            var result = await service.Delete(key);

            //Assert
            using (Assert.EnterMultipleScope())
            {
                repository.Verify(x => x.GetDatabase(It.IsAny<int>(), It.IsAny<object>()).KeyDeleteAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()), Times.Once);
                Assert.That(result, Is.True);
            }
        }

        [Test]
        public async Task DeleteField_Verify()
        {
            //Arrange
            var repository = new Mock<IConnectionMultiplexer>();
            repository.Setup(x => x.GetDatabase(It.IsAny<int>(), It.IsAny<object>()).HashDeleteAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<CommandFlags>())).Returns(Task.FromResult(true));
            var service = new RedisService(repository.Object);

            //Act
            var result = await service.DeleteField(key, value);

            //Assert
            using (Assert.EnterMultipleScope())
            {
                repository.Verify(x => x.GetDatabase(It.IsAny<int>(), It.IsAny<object>()).HashDeleteAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<CommandFlags>()), Times.Once);
                Assert.That(result, Is.True);
            }
        }

        [Test]
        public void ConvertHashEntryToObject_Verify()
        {
            //Arrange
            var hashedTest = serviceConversion.ObjectToHashEntry(test);
            var repository = new Mock<IConnectionMultiplexer>();
            var service = new RedisService(repository.Object);

            //Act
            var result = service.HashEntryToObject<Test>(hashedTest);

            //Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result?.Key, Is.EqualTo(key));
                Assert.That(result?.Value, Is.EqualTo(value));
            }
        }

        private class Test
        {
            public string? Key { get; set; }
            public string? Value { get; set; }
        }
    }
}