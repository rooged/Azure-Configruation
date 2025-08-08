using Roo.Azure.Configuration.Common.Http;
using SampleWebApp.Models;
using SampleWebApp.Models.Enums;

namespace SampleWebApp.Services
{
    public interface IExampleService
    {
        public Task<int> PrivateExample();
        public Task<HttpResponseMessage> PublicExample(ExampleModel input);
        public Task<ExampleModel> ObjectExample(int number);
        public Task<int> Example(ExampleModel input);
        public Task<string> AnotherExample(string input);
    }

    public class ExampleService : IExampleService
    {
        private readonly IRooHttpClient _rooHttpClient;

        private readonly string _privateExampleEndpoint = "api/privateBackend";
        private readonly string _publicExampleEndpoint = "api/privateBackend/example";
        private readonly string _objectExampleEndpoint = "api/privateBackend/object/{number}";
        private readonly string _exampleEndpoint = "api/exampleBackend";
        private readonly string _anotherExampleEndpoint = "api/exampleBackend/another?input={input}";

        public ExampleService(IRooHttpClient rooHttpClient)
        {
            _rooHttpClient = rooHttpClient;
        }

        public async Task<int> PrivateExample()
        {
            return await _rooHttpClient.GetAsync<int>(_privateExampleEndpoint, BaseUrls.ExampleBackend);
        }

        public async Task<HttpResponseMessage> PublicExample(ExampleModel input)
        {
            return await _rooHttpClient.PostAsync(_publicExampleEndpoint, BaseUrls.ExampleBackend, input);
        }

        public async Task<ExampleModel> ObjectExample(int number)
        {
            var endpoint = _objectExampleEndpoint.Replace("{number}", number.ToString());
            return await _rooHttpClient.GetAsync<ExampleModel>(endpoint, BaseUrls.ExampleBackend);
        }

        public async Task<int> Example(ExampleModel input)
        {
            return await _rooHttpClient.PostAsync<int>(_exampleEndpoint, BaseUrls.ExampleBackend, input);
        }

        public async Task<string> AnotherExample(string input)
        {
            var endpoint = _anotherExampleEndpoint.Replace("{input}", input);
            return await _rooHttpClient.GetAsync<string>(endpoint, BaseUrls.ExampleBackend);
        }
    }
}
