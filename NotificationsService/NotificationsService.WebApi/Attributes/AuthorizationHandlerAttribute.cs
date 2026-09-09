using Microsoft.AspNetCore.Authorization;

namespace NotificationsService.WebApi.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class AuthorizationHandlerAttribute : AuthorizeAttribute
{
    public AuthorizationHandlerAttribute()
    {
        AuthenticationSchemes = "CheckIdentityToken";
    }
}