namespace transactionnotes.Web.Services
{
    public class OrganisationApiClient(HttpClient httpClient)
    {
        public async Task<OrganisationMembershipResponse?> GetUserOrganisationAsync()
        {
            // Replace with your actual backend API endpoint
            var response = await httpClient.GetAsync("/api/v1/organisation");
            if (!response.IsSuccessStatusCode)
                return null;
            return await response.Content.ReadFromJsonAsync<OrganisationMembershipResponse>();
        }
    }
}
