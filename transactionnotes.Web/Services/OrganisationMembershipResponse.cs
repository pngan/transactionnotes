namespace transactionnotes.Web.Services;

public class OrganisationMembershipResponse
{
    public Guid[] Tenants { get; set; }
    public Guid? CurrentTenant { get; set; }
}