using Microsoft.AspNetCore.Mvc;

namespace SampleWebApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HiddenController : ControllerBase
    {
        public HiddenController() { }

        [HttpGet]
        public int HiddenMethod()
        {
            return 1;
        }
    }
}
