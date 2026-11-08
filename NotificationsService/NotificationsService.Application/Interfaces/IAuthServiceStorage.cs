using NotificationsService.Application.Models;

namespace NotificationsService.Application.Interfaces;

public interface IAuthServiceStorage
{
    Task<TokenVerifyResponse?> VerifyAsync(Guid accessToken, CancellationToken cancellationToken = default);
}