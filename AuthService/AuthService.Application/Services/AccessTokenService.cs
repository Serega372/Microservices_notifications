using AuthService.Application.Interfaces;
using AuthService.Application.Models.Auth;
using AuthService.Application.Models.ErrorModel;
using AuthService.Core.Entities;
using AuthService.Core.Interfaces;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace AuthService.Application.Services;

public sealed class AccessTokenService : IAccessTokenService
{
    private readonly IAccessTokenRepository _accessTokenRepository;

    private readonly IUserRepository _userRepository;

    private readonly IPermissionRepository _permissionRepository;

    private readonly ILogger<AccessTokenService> _logger;

    public AccessTokenService(
        IAccessTokenRepository accessTokenRepository,
        IUserRepository userRepository,
        IPermissionRepository permissionRepository,
        ILogger<AccessTokenService> logger)
    {
        _accessTokenRepository = accessTokenRepository;
        _userRepository = userRepository;
        _permissionRepository = permissionRepository;
        _logger = logger;

        _logger.LogInformation($"{GetType().Name} was initialized");
    }

    public async Task<(TokenVerifyResponse? response, ErrorModel? errorModel)> VerifyAsync(TokenVerifyRequest request, CancellationToken cancellationToken = default)
    {
        Expression<Func<AccessTokenEntity, bool>> accessTokenPredicate = entity =>
            entity.Token == request.AccessToken
            && entity.RevocationDate == null
            && entity.ExpirationDate > DateTime.UtcNow;

        var foundAccessToken = (await _accessTokenRepository.GetAsync(accessTokenPredicate, cancellationToken: cancellationToken)).FirstOrDefault();
        if (foundAccessToken is null)
        {
            _logger.LogWarning($"Active [{nameof(AccessTokenEntity)}] [{request.AccessToken}] not found");
            return (null, new ErrorModel($"[{nameof(AccessTokenEntity)}] not active or not exists"));
        }

        var foundUserByAccessToken = await _userRepository.GetByIdAsync(foundAccessToken.UserId, cancellationToken: cancellationToken);
        if (foundUserByAccessToken is null)
        {
            _logger.LogWarning($"[{nameof(UserEntity)}] with id [{foundAccessToken.UserId}] not found by access token [{foundAccessToken.Token}]");
            return (null, new ErrorModel($"[{nameof(UserEntity)}] with requested session not found"));
        }

        var permissionsByRole = await _permissionRepository.GetByRoleIdAsync(foundUserByAccessToken.RoleId, cancellationToken: cancellationToken);
        return (new TokenVerifyResponse
        {
            UserId = foundUserByAccessToken.Id,
            ExpirationDate = foundAccessToken.ExpirationDate,
            Permissions = permissionsByRole
        }, null);
    }
}