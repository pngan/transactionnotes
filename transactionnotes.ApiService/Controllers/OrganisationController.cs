using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using transactionnotes.ApiService.ApiTypes.Response;

namespace transactionnotes.ApiService.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class OrganisationController : ControllerBase
    {
        [HttpGet]
        [Authorize]
        public OrganisationMembershipResponse Get()
        {
            return new OrganisationMembershipResponse([], null);
            //var tenant1 = new Guid("8EF7D8BF-6CFC-4AD2-8B92-4C7584CF84ED");
            //var tenant2 = new Guid("68250AE3-D739-46BA-B623-F2E6E0395BB5");
            //Guid[] tenants = [tenant1, tenant2];
            //return new OrganisationMembershipResponse(tenants, tenants[0]);
        }
    }
}