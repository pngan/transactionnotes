using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using transactionnotes.ApiService.ApiTypes.Response;

namespace transactionnotes.ApiService.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        [HttpGet]
        [Authorize]
        public UserResponse Get()
        {

            return new UserResponse(Guid.NewGuid(), "John", "Doe", "john.doe@example.com");
        }
    }
}