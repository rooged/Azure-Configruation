using Microsoft.OpenApi.Extensions;
using Roo.Azure.Configuration.Common.ServiceExceptions;

namespace Roo.Azure.Configuration.UnitTests
{
    public class ServiceExceptionConverterTests
    {
        //Common
        private string transactionId = "transactionId";

        [Test]
        public void ConvertToServiceException_Verify()
        {
            //Arrange
            var exceptionMessage = "Exception Conversion Test";
            var exception = new InvalidOperationException(exceptionMessage);

            //Act
            var serviceException = exception.ConvertToServiceException(transactionId);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(serviceException.Error.Code, Is.EqualTo(ErrorCode.InvalidOperation));
                Assert.That(serviceException.Error.CodeName, Is.EqualTo(ErrorCode.InvalidOperation.GetDisplayName()));
                Assert.That(serviceException.Error.Message, Is.EqualTo(exceptionMessage));
                Assert.That(serviceException.Error.TransactionId, Is.EqualTo(transactionId));
                Assert.That(serviceException.Message, Is.EqualTo("Exception of type 'Roo.Azure.Configuration.Common.ServiceExceptions.ServiceException' was thrown."));
                Assert.That(serviceException.InnerException, Is.Not.Null);
                Assert.That(serviceException.InnerException?.Message, Is.EqualTo(exceptionMessage));
            });
        }

        [Test]
        public void ConvertFromServiceException_Verify()
        {
            //Arrange
            var exceptionMessage = "Exception Conversion Test";
            var serviceError = new ServiceError(ErrorCode.InvalidOperation, exceptionMessage, null, transactionId);
            var serviceException = new ServiceException(serviceError, new ArgumentException());

            //Act
            var exception = serviceException.ConvertFromServiceException();

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(exception.GetType(), Is.EqualTo(typeof(InvalidOperationException)));
                Assert.That(exception.Message, Is.EqualTo(exceptionMessage));
                Assert.That(exception.InnerException, Is.Not.Null);
                Assert.That(exception.InnerException?.GetType(), Is.EqualTo(typeof(ArgumentException)));
            });
        }
    }
}