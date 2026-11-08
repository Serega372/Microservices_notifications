using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NotificationsService.Application.Interfaces;
using NotificationsService.Application.Models;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace NotificationsService.Infrastructure.ExternalServices;

public sealed class AuthorizationHandlerService(
    IMemoryCacheStorage memoryCacheStorage,
    IOptionsMonitor<AuthenticationSchemeOptions> options, 
    ILoggerFactory logger, 
    UrlEncoder encoder, 
    ISystemClock clock,
    IAuthServiceStorage authServiceStorage) 
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder, clock)
{
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var authHeaderName = "X-Auth-Token";
        if (!Request.Headers.TryGetValue(authHeaderName, out var authHeaderValue))
        {
            return AuthenticateResult.Fail("No authentication header provided");
        }

        if (!Guid.TryParse(authHeaderValue, out var accessToken))
        {
            return AuthenticateResult.Fail("Access token not valid");
        }

        var verifyResultFromCache = memoryCacheStorage.GetValue<Guid, TokenVerifyResponse>(accessToken);
        if (verifyResultFromCache is null) 
        {
            var verifyResultFromAuthService = await authServiceStorage.VerifyAsync(accessToken);
            if (verifyResultFromAuthService is null)
            {
                return AuthenticateResult.Fail("Verify from auth service failed");
            }

            var maxTimeToLive = CalculateTokenMaxTimeToLive(verifyResultFromAuthService.ExpirationDate);
            memoryCacheStorage.SetValue(accessToken, verifyResultFromAuthService, maxTimeToLive);
            verifyResultFromCache = verifyResultFromAuthService;
        }

        var claims = new List<Claim>
        {
            new (ClaimTypes.NameIdentifier, verifyResultFromCache.UserId.ToString()),
        };

        if (verifyResultFromCache.Permissions.Any())
        {
            foreach (var permission in verifyResultFromCache.Permissions) 
            {
                claims.Add(new Claim("permission", permission));
            }
        }

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var properties = new AuthenticationProperties();

        properties.StoreTokens([new AuthenticationToken { Name = "accessToken", Value = verifyResultFromCache.AccessToken.ToString() }]);
        properties.ExpiresUtc = DateTime.UtcNow + CalculateTokenMaxTimeToLive(verifyResultFromCache.ExpirationDate);

        return AuthenticateResult.Success(new(principal, properties, Scheme.Name));
    }

    private static TimeSpan CalculateTokenMaxTimeToLive(DateTime tokenExpirationTime)
    {
        var remainingTimeToExpiration = tokenExpirationTime - DateTime.UtcNow;
        return remainingTimeToExpiration > TimeSpan.FromMinutes(5) ? TimeSpan.FromMinutes(5) : remainingTimeToExpiration;
    }
}