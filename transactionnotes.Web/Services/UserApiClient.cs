using System.Net.Http.Json;

namespace transactionnotes.Web.Services
{
    public class UserApiClient
    {
        private readonly HttpClient _httpClient;

        public UserApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<UserResponse?> GetUserAsync()
        {
            // Call the UserController's GET method
            var response = await _httpClient.GetAsync("/api/v1/user");
            if (!response.IsSuccessStatusCode)
                return null;
                
            return await response.Content.ReadFromJsonAsync<UserResponse>();
        }
    }
}