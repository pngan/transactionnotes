using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace central.api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class PingController : ControllerBase
{
    
    [HttpGet()]
    public string Get()
    {
        return "Central Pong";
    }

    [HttpGet("secure")]
    [Authorize]
    public string SecurePing()
    {
        return "Central Secure Pong";
    }
}
