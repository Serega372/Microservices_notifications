using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text.Encodings.Web;

namespace NotificationsService.Infrastructure.ExternalServices;

public sealed class AuthorizationHandlerService(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) 
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder, clock)
{
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var authHeaderName = "X-Auth-Token";
        if (!Request.Headers.TryGetValue(authHeaderName, out var authHeaderValue))
        {
            return AuthenticateResult.Fail("No authentication header provided");
        }

        
    }
}