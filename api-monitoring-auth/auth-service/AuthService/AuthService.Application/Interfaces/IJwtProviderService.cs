using AuthService.Core.Entities;

namespace AuthService.Application.Interfaces;

public interface IJwtProviderService
{
    string GenerateToken(User user);

    Task<string?> GenerateRefreshToken(long userId, CancellationToken cancellationToken = default);
}