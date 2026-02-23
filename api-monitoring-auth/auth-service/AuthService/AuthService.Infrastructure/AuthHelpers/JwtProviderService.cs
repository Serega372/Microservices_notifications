using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthService.Application.Configs;
using AuthService.Application.Interfaces;
using AuthService.Core.Entities;
using AuthService.Core.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AuthService.Infrastructure.AuthHelpers;

public sealed class JwtProviderService(
    IOptions<JwtConfig> jwtOptions,
    IAuthRepository _authRepository) 
    : IJwtProviderService
{
    public string GenerateToken(User user)
    {
        var claims = new List<Claim>
        {
            new("Id", user.Id.ToString()),
            new("Role", user.Role.ToString())
        };
        
        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Value.SecretKey)),
            SecurityAlgorithms.HmacSha256);

        var jwtToken = new JwtSecurityToken(
            claims: claims,
            signingCredentials: signingCredentials,
            expires: DateTime.UtcNow.AddMinutes(jwtOptions.Value.ExpiresMinutes));
        
        return new JwtSecurityTokenHandler().WriteToken(jwtToken);
    }

    public async Task<string?> GenerateRefreshToken(
        long userId,
        CancellationToken cancellationToken = default)
    {
        var refreshTokenExpiresMinutes = jwtOptions.Value.RefreshTokenExpiresMinutes;
        var refreshToken = new RefreshToken
        {
            Token = Guid.NewGuid().ToString(),
            ExpiresAt = DateTime.UtcNow.AddMinutes(refreshTokenExpiresMinutes),
            UserId = userId
        };

        var existToken = await _authRepository.GetByUserIdAsync(userId, cancellationToken);
        if (existToken?.Token is not null)
        {
            var existTokenDeleted = await _authRepository.DeleteRefreshTokenAsync(existToken.Token, cancellationToken);
            if (!existTokenDeleted)
            {
                return null;
            }
        }
        
        var refreshTokenCreated = await _authRepository.AddRefreshTokenAsync(refreshToken, cancellationToken);
        
        return !refreshTokenCreated 
            ? null 
            : refreshToken.Token;
    }
}