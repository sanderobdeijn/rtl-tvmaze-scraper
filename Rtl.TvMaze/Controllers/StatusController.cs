using Microsoft.AspNetCore.Mvc;

namespace Rtl.TvMaze.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatusController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Status Ok");
        }
    }
}