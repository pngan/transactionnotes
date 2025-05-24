using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using transactionnotes.ApiService.Authorization;

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

        [HttpGet("secure")]
        [Authorize]
        public string SecurePing()
        {
            return "Secure Pong";
        }

        [HttpGet("allowed")]
        [Authorize(Policy = "CanWrite")]
        //[RequiresReadPermission]
        public string AllowedPing()
        {
            return "Allowed Pong";
        }
    }
}
