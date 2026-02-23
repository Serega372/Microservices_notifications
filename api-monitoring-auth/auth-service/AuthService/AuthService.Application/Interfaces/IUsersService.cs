using AuthService.Application.Models;

namespace AuthService.Application.Interfaces;

public interface IUsersService
{
    Task<long?> CreateUserAsync(RegisterUserRequest request, bool isAdmin = false,
        CancellationToken cancellationToken = default);
    
    Task<LoginUserResponse?> LoginAsync(LoginUserRequest request, CancellationToken cancellationToken = default);
    
    Task<RefreshTokenResponse?> ValidateRefreshTokenAsync(string token, CancellationToken cancellationToken = default);
}