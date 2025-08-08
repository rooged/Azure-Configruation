using Microsoft.AspNetCore.Mvc;
using Roo.Azure.Configuration.Common.Utilities.Extensions;
using SampleWebApp.Models;
using SampleWebApp.Services;

namespace SampleWebApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ExampleController : ControllerBase
    {
        private readonly IExampleService _exampleService;

        public ExampleController(IExampleService exampleService)
        {
            _exampleService = exampleService;
        }

        [HttpGet]
        public async Task<int> PrivateExample()
        {
            return await _exampleService.PrivateExample();
        }

        [HttpPost]
        public async Task<HttpResponseMessage> PublicExample(ExampleModel input)
        {
            return await _exampleService.PublicExample(input);
        }

        [HttpGet("object/{number}")]
        public async Task<ExampleModel> ObjectExample(int number)
        {
            var result = await _exampleService.ObjectExample(number);
            return result;
        }

        [HttpPost("another")]
        public async Task<int> Example()
        {
            var random = new Random().Next();
            return await _exampleService.Example(new() { Id = random, Name = $"Object {random}", PacificTime = DateTime.UtcNow.ToPstFromUtc() });
        }

        [HttpGet("another")]
        public async Task<string> AnotherExample(string input)
        {
            return await _exampleService.AnotherExample(input);
        }

        [HttpPost("hide")]
        public HiddenModel HiddenClassMethod(HiddenModel input)
        {
            return input;
        }
    }
}
