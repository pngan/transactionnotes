using Microsoft.AspNetCore.Components;

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

        public void HandleError(Exception ex, string component)
        {
            if (ex is HttpRequestException httpEx)
            {
                if (httpEx.StatusCode == System.Net.HttpStatusCode.Unauthorized || httpEx.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    _logger.LogWarning("Unauthorized access attempt in {Component}", component);
                    _navigationManager.NavigateTo($"/unauthorized?returnUrl={Uri.EscapeDataString(_navigationManager.Uri)}");
                    //_navigationManager.NavigateTo($"/");
                }
            }
        }
    }
}
