using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Roo.Azure.Configuration.Common.Models;
using Roo.Azure.Configuration.Common.ServiceExceptions;

namespace Roo.Azure.Configuration.UnitTests
{
    public class ServiceExceptionFilterTests
    {
        //Common
        private readonly string transactionId = "transactionId";

        [Test]
        public async Task ExceptionFilter_Verify()
        {
            //Arrange
            var actionContext = new ActionContext();
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers.TryAdd(Constants.TransactionIdHeaderName, transactionId);
            using var memoryStream = new MemoryStream();
            httpContext.Response.Body = memoryStream;
            actionContext.HttpContext = httpContext;
            actionContext.RouteData = new Microsoft.AspNetCore.Routing.RouteData();
            actionContext.ActionDescriptor = new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor();
            var context = new ExceptionContext(actionContext, new List<IFilterMetadata>());
            var message = "Exception Test";
            context.Exception = new IndexOutOfRangeException(message);
            var filter = new ServiceExceptionFilter();

            //Act
            filter.OnException(context);
            context.HttpContext.Response.Headers.TryGetValue(Constants.TransactionIdHeaderName, out var transactionIdHeader);
            memoryStream.Seek(0, SeekOrigin.Begin);
            var response = await new StreamReader(memoryStream).ReadToEndAsync();
            memoryStream.Dispose();

            //Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(transactionIdHeader.First(), Is.EqualTo(transactionId));
                Assert.That(context.HttpContext.Response.StatusCode, Is.EqualTo(544));
                Assert.That(response, Is.EqualTo("{\"Error\":{\"Code\":544,\"CodeName\":\"IndexOutOfRange\",\"Message\":\"Exception Test\",\"Details\":{\"Type\":\"System.IndexOutOfRangeException\",\"BaseMessage\":\"Exception Test\",\"Source\":\"\",\"Method\":\"\",\"StackTrace\":\"\",\"HelpLink\":\"\"},\"TransactionId\":\"transactionId\"},\"InnerException\":{\"ClassName\":\"System.IndexOutOfRangeException\",\"Message\":\"Exception Test\",\"Data\":null,\"InnerException\":null,\"HelpURL\":null,\"StackTraceString\":null,\"RemoteStackTraceString\":null,\"RemoteStackIndex\":0,\"ExceptionMethod\":null,\"HResult\":-2146233080,\"Source\":null,\"WatsonBuckets\":null},\"Message\":\"Exception of type 'Roo.Azure.Configuration.Common.ServiceExceptions.ServiceException' was thrown.\",\"Data\":{},\"HelpLink\":null,\"Source\":null,\"HResult\":-2146233088,\"StackTrace\":null}"));
            }
        }
    }
}