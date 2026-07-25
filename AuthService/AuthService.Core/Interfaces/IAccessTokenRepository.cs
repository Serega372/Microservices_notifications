using AuthService.Core.Entities;

namespace AuthService.Core.Interfaces;

public interface IAccessTokenRepository : IBaseRepository<AccessTokenEntity>
{
    Task<AccessTokenEntity> CreateAsync(long userId, long refreshTokenId, CancellationToken cancellationToken = default);
}