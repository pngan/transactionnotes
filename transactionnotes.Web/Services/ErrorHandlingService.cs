    // Exception to signal user logout/navigation for error handling

    using Microsoft.AspNetCore.Components;
    using Exception = System.Exception;
    using HttpRequestException = System.Net.Http.HttpRequestException;
    using Uri = System.Uri;

namespace transactionnotes.Web.Services
{
    public class ErrorHandlingService
    {
        private readonly NavigationManager _navigationManager;
        private readonly ILogger<ErrorHandlingService> _logger;

        public ErrorHandlingService(NavigationManager navigationManager,
            ILogger<ErrorHandlingService> logger)
        {
            _navigationManager = navigationManager;
            _logger = logger;
        }


        public async Task HandleErrorAsync(HttpResponseMessage? response, string component)
        {
            if (response == null)
                return;

            // Only handle 401/403
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized ||
                response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                string? problemDetail = null;
                try
                {
                    if (response.Content.Headers.ContentType?.MediaType == "application/problem+json")
                    {
                        var problem = await response.Content.ReadFromJsonAsync<ProblemDetailsPayload>();
                        if (problem != null)
                        {
                            problemDetail = problem.Detail ?? problem.Title;
                        }
                    }
                }
                catch { /* Ignore JSON parse errors */ }

                var message = problemDetail ?? "You are not authorized to access this resource.";
                _logger.LogWarning("Unauthorized/Forbidden access in {Component}: {Message}", component, message);

                // Use a Task.Run to ensure the navigation happens immediately
                //_navigationManager.NavigateTo("/error", forceLoad: true);

                // Throw to break the await chain and prevent stuck loading UI
                //throw new UserLoggedOutException("Navigated to error page due to authentication error.");
            }
        }

        // For backward compatibility
        public void HandleError(Exception ex, string component)
        {
            if (ex is HttpRequestException httpEx)
            {
                if (httpEx.StatusCode == System.Net.HttpStatusCode.Unauthorized || httpEx.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    _logger.LogWarning("Unauthorized access attempt in {Component}", component);
                    _navigationManager.NavigateTo($"/unauthorized?returnUrl={Uri.EscapeDataString(_navigationManager.Uri)}");
                }
            }
        }

        private class ProblemDetailsPayload
        {
            public string? Type { get; set; }
            public string? Title { get; set; }
            public int? Status { get; set; }
            public string? Detail { get; set; }
            public string? Instance { get; set; }
        }
    }
}
