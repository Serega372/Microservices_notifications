using AuthService.Core.Entities;

namespace AuthService.Core.Interfaces;

public interface IAuthRepository
{
    Task<bool> AddRefreshTokenAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default);

    Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);
    
    Task<RefreshToken?> GetByUserIdAsync(long userId, CancellationToken cancellationToken = default);

    Task<bool> DeleteRefreshTokenAsync(string token, CancellationToken cancellationToken = default);
}