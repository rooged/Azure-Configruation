using Microsoft.AspNetCore.Mvc;
using Roo.Azure.Configuration.Common.Utilities.Extensions;
using SampleWebAppBackend.Models;

namespace SampleWebAppBackend.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class PrivateBackendController : ControllerBase
    {
        public PrivateBackendController() { }

        [HttpGet]
        public int PrivateExample()
        {
            var random = new Random();
            return random.Next();
        }

        [ApiExplorerSettings(GroupName = "public")]
        [HttpPost("example")]
        public int PublicExample(ExampleObject input)
        {
            return input.Id;
        }

        [HttpGet("object/{number}")]
        public ExampleObject ObjectExample(int number)
        {
            var result = new ExampleObject()
            {
                Id = number,
                Name = $"Sample Object {number}",
                PacificTime = DateTime.Now.ToPst()
            };
            return result;
        }
    }
}
