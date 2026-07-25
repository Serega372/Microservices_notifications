using AuthService.Application.Interfaces;
using AuthService.Application.Models.Auth;
using AuthService.Application.Models.ErrorModel;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.WebApi.Controllers;

[Route("api/v1/auth")]
[ApiController]
public sealed class AuthController : ControllerBase
{
    private readonly ILogger<AuthController> _logger;

    private readonly IAuthService _authService;

    private readonly IAccessTokenService _accessTokenService;

    private readonly IRefreshTokenService _refreshTokenService;

    public AuthController(
        IAuthService authService,
        IAccessTokenService accessTokenService,
        IRefreshTokenService refreshTokenService,
        ILogger<AuthController> logger)
    {
        _authService = authService;
        _accessTokenService = accessTokenService;
        _refreshTokenService = refreshTokenService;
        _logger = logger;

        _logger.LogInformation($"{GetType().Name} was initialized");
    }

    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RegisterResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorModel))]
    public async Task<IActionResult> RegisterAsync([FromBody] RegisterRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var (tokenPair, errorModel) = await _authService.RegisterAsync(request, cancellationToken: cancellationToken);
            if (tokenPair is null)
            {
                _logger.LogError($"User not registered with email [{request.Email}]");
                return BadRequest(errorModel);
            }

            return Ok(tokenPair);
        }
        catch (Exception ex)
        {
            var message = $"Exception occured while register user with email [{request.Email}]";
            _logger.LogError(ex, message);
            return BadRequest(new ErrorModel(message, ex.Message));
        }
    }

    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LoginResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorModel))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorModel))]
    public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var (tokenPair, errorModel) = await _authService.LoginAsync(request, cancellationToken: cancellationToken);
            if (tokenPair is null)
            {
                _logger.LogError($"User login failure with email [{request.Email}]");
                return Unauthorized(errorModel);
            }

            return Ok(tokenPair);
        }
        catch (Exception ex)
        {
            var message = $"Exception occured while login user with email [{request.Email}]";
            _logger.LogError(ex, message);
            return BadRequest(new ErrorModel(message, ex.Message));
        }
    }

    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorModel))]
    public async Task<IActionResult> LogoutAsync([FromBody] LogoutRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var errorModel = await _authService.Logout(request, cancellationToken: cancellationToken);
            if (errorModel is not null)
            {
                _logger.LogError($"User logout failure with refresh token [{request.RefreshToken}], maybe already logout");
                return BadRequest(errorModel);
            }

            return Ok("Success");
        }
        catch (Exception ex)
        {
            var message = $"Exception occured while logout user";
            _logger.LogError(ex, message);
            return BadRequest(new ErrorModel(message, ex.Message));
        }
    }

    [HttpPost("verify")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LoginResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorModel))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorModel))]
    public async Task<IActionResult> VerifyTokenAsync([FromBody] TokenVerifyRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var (tokenInfo, errorModel) = await _accessTokenService.VerifyAsync(request, cancellationToken: cancellationToken);
            if (tokenInfo is null)
            {
                _logger.LogError($"Verify user with token [{request.AccessToken}] is failure");
                return Unauthorized(errorModel);
            }

            return Ok(tokenInfo);
        }
        catch (Exception ex)
        {
            var message = $"Exception occured while verify user by token";
            _logger.LogError(ex, message);
            return BadRequest(new ErrorModel(message, ex.Message));
        }
    }

    [HttpPost("refresh")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LoginResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorModel))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorModel))]
    public async Task<IActionResult> RefreshTokenAsync([FromBody] TokenRefreshRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var (tokenInfo, errorModel) = await _refreshTokenService.RefreshTokenAsync(request, cancellationToken);
            if (tokenInfo is null)
            {
                _logger.LogError($"Refresh user session with token [{request.RefreshToken}] is failure");
                return Unauthorized(errorModel);
            }

            return Ok(tokenInfo);
        }
        catch (Exception ex)
        {
            var message = $"Exception occured while refresh session by token";
            _logger.LogError(ex, message);
            return BadRequest(new ErrorModel(message, ex.Message));
        }
    }
}