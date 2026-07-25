using AuthService.Core.Entities;
using AuthService.Core.Interfaces;
using AuthService.Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AuthService.Infrastructure.Repositories;

public sealed class AccessTokenRepository(
    DatabaseContext context,
    ILogger<AccessTokenEntity> logger)
    : BaseRepository<AccessTokenEntity>(context, logger),
    IAccessTokenRepository
{
    public async Task<AccessTokenEntity> CreateAsync(long userId, long refreshTokenId, CancellationToken cancellationToken = default)
    {
        var accessTokenEntityToCreate = new AccessTokenEntity
        {
            Id = default,
            Name = string.Empty,
            Token = Guid.NewGuid(),
            ExpirationDate = DateTime.UtcNow.AddMinutes(30),
            UserId = userId,
            RefreshTokenId = refreshTokenId
        };

        return await AddAsync(accessTokenEntityToCreate, cancellationToken);
    }
}