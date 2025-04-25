using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace transactionnotes.ApiService.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class PingController : ControllerBase
    {
        [HttpGet]
        public string Get()
        {
            return "Pong";
        }
    }
}
