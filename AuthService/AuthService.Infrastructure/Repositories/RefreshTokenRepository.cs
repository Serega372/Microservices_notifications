using AuthService.Application.Models.RefreshTokens;
using AuthService.Core.Entities;
using AuthService.Core.Interfaces;
using AuthService.Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AuthService.Infrastructure.Repositories;

public sealed class RefreshTokenRepository(
    DatabaseContext context, 
    ILogger<RefreshTokenEntity> logger) 
    : BaseRepository<RefreshTokenEntity>(context, logger),
    IRefreshTokenRepository
{
    public async Task<RefreshTokenEntity> CreateAsync(long userId, CancellationToken cancellationToken = default)
    {
        var refreshTokenEntityToCreate = new RefreshTokenEntity
        {
            Id = default,
            Name = string.Empty,
            Token = Guid.NewGuid(),
            ExpirationDate = DateTime.UtcNow.AddDays(1),
            UserId = userId,
        };

        return await AddAsync(refreshTokenEntityToCreate, cancellationToken);
    }

    public async Task<RefreshTokenEntity?> GetWithAccessTokensAsync(Guid refreshToken, bool asNoTracking = true, 
        CancellationToken cancellationToken = default)
    {
        var query = asNoTracking
            ? DbSet.AsNoTracking()
            : DbSet;

        query = query
            .Where(entity => entity.Token == refreshToken && entity.RevocationDate == null && entity.ExpirationDate > DateTime.UtcNow)
            .Include(entity => entity.AccessTokens
                .Where(accessToken => accessToken.RevocationDate == null && accessToken.ExpirationDate > DateTime.UtcNow));

        return await query.FirstOrDefaultAsync(cancellationToken);
    }
}