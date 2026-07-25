using AuthService.Application.Models.Auth;
using AuthService.Application.Models.ErrorModel;
using AuthService.Application.Models.RefreshTokens;

namespace AuthService.Application.Interfaces;

public interface IRefreshTokenService
{
    Task<(TokenRefreshResponse? response, ErrorModel? errorModel)> RefreshTokenAsync(TokenRefreshRequest request, CancellationToken cancellationToken = default);

    Task<RefreshTokenDto?> RevokeAsync(Guid refreshToken, CancellationToken cancellationToken = default);

    Task<RefreshTokenDto?> CreateAsync(long userId, CancellationToken cancellationToken = default);
}