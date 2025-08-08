using Microsoft.AspNetCore.Mvc;
using SampleWebAppBackend.Models;

namespace SampleWebAppBackend.Controllers
{

    [ApiExplorerSettings(GroupName = "example")]
    [ApiController]
    [Route("[controller]")]
    public class ExampleBackendController : ControllerBase
    {
        public ExampleBackendController() { }

        [HttpPost]
        public int Example(ExampleObject input)
        {
            return input.Id;
        }

        [HttpGet("another")]
        public string AnotherExample(string input)
        {
            return input;
        }
    }
}
