using AuthService.Application.Models.Auth;
using AuthService.Application.Models.ErrorModel;

namespace AuthService.Application.Interfaces;

public interface IAccessTokenService
{
    Task<(TokenVerifyResponse? response, ErrorModel? errorModel)> VerifyAsync(TokenVerifyRequest request, CancellationToken cancellationToken = default);
}