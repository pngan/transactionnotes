using Microsoft.AspNetCore.Mvc;

namespace central.api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class PingController : ControllerBase
{
    [HttpGet]
    public string Get()
    {
        return "Pong";
    }
}
