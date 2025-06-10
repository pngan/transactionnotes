namespace transactionnotes.ApiService.ApiTypes.Response;

public class OrganisationMembershipResponse(Guid[] tenants, Guid? currentTenant)
{
    public Guid[] Tenants { get; set; } = tenants;
    public Guid? CurrentTenant { get; set; } = currentTenant;
}