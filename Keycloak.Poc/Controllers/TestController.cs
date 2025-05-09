
using Microsoft.AspNetCore.Mvc;

namespace Keycloak.Poc.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        [HttpPost]
        public IActionResult TestNginx([FromBody] string value)
        {
           Console.WriteLine($"The value ist {value}");
           return Ok();
        }

        [HttpGet]
        [Route("test")]
        public  IActionResult TestGet()
        {

            return Ok("Succeeded");
        }
    }
}
