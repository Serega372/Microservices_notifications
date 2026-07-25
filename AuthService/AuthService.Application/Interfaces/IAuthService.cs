using AuthService.Application.Models.Auth;
using AuthService.Application.Models.ErrorModel;

namespace AuthService.Application.Interfaces;

public interface IAuthService
{
    Task<(RegisterResponse? response, ErrorModel? errorModel)> RegisterAsync(RegisterRequest request, bool isAdmin = false, CancellationToken cancellationToken = default);

    Task<(LoginResponse? response, ErrorModel? errorModel)> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);

    Task<ErrorModel?> Logout(LogoutRequest request, CancellationToken cancellationToken = default);
}