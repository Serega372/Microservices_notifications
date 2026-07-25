using AuthService.Core.Entities;

namespace AuthService.Core.Interfaces;

public interface IRefreshTokenRepository : IBaseRepository<RefreshTokenEntity>
{
    Task<RefreshTokenEntity> CreateAsync(long userId, CancellationToken cancellationToken = default);

    Task<RefreshTokenEntity?> GetWithAccessTokensAsync(Guid refreshToken, bool asNoTracking = true,
        CancellationToken cancellationToken = default);
}