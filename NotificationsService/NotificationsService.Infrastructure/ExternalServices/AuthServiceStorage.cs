using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NotificationsService.Application.Configs;
using NotificationsService.Application.Interfaces;
using NotificationsService.Application.Models;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace NotificationsService.Infrastructure.ExternalServices;

public sealed class AuthServiceStorage : IAuthServiceStorage
{
    private readonly ILogger<AuthServiceStorage> _logger;

    private readonly HttpClient _httpClient;

    private readonly AuthServiceOptions _authServiceOptions;

    public AuthServiceStorage(
        HttpClient httpClient,
        ILogger<AuthServiceStorage> logger,
        IOptions<ExternalServicesConfig> externalServicesConfig)
    {
        _httpClient = httpClient;
        _logger = logger;
        _authServiceOptions = externalServicesConfig.Value.AuthServiceOptions;

        _logger.LogInformation($"{GetType().Name} was initialized");
    }

    public async Task<TokenVerifyResponse?> VerifyAsync(Guid accessToken, CancellationToken cancellationToken = default)
    {
        var verifyUri = $"{_authServiceOptions.VerifyPath}";
        var requestContent = new StringContent(JsonSerializer.Serialize(new { AccessToken = accessToken }), Encoding.UTF8, "application/json");

        var responseMessage = await _httpClient.PostAsync(verifyUri, requestContent, cancellationToken);
        if (!responseMessage.IsSuccessStatusCode)
        {
            // ...
        }

        return await responseMessage.Content.ReadFromJsonAsync<TokenVerifyResponse>(cancellationToken);
    }
}