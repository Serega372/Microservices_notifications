using AuthService.Application.Interfaces;
using AuthService.Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.WebApi.Controllers;

[ApiController]
[Route("api/users")]
public sealed class UsersController : ControllerBase
{
    private readonly ILogger<UsersController> _logger;
    
    private readonly IUsersService _usersService;

    public UsersController(
        ILogger<UsersController> logger,
        IUsersService usersService)
    {
        _logger = logger;
        _usersService = usersService;
        
        _logger.LogInformation($"{GetType().Name} was initialized");
    }
    
    [AllowAnonymous]
    [HttpPost("register")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RegisterUserAsync(
        [FromBody] RegisterUserRequest request,
        CancellationToken cancellationToken = default)
    {
        var message = string.Empty;
        
        try
        {
            var createdUserId = await _usersService.CreateUserAsync(request, cancellationToken: cancellationToken);
            if (createdUserId is null)
            {
                _logger.LogWarning($"User not created with {request.Username} in {GetType().Name}");
                return BadRequest("Could not create user");
            }

            message = $"User successfully created with [id={createdUserId}]";
            _logger.LogInformation(message);
            
            return Ok(message);
        }
        catch (OperationCanceledException)
        {
            message = $"Operation was cancelled in {DateTime.UtcNow}";
            _logger.LogInformation(message);
            
            return BadRequest(message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return BadRequest($"An exception occured in {GetType().Name}");
        }
    }

    [Authorize]
    [HttpGet("verify")]
    public IActionResult VerifyToken()
    {
        return Ok();
    }

    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> LoginAsync(
        [FromBody] LoginUserRequest request,
        CancellationToken cancellationToken = default)
    {
        var message = string.Empty;

        try
        {
            var token = await _usersService.LoginAsync(request, cancellationToken);
            if (token is null)
            {
                _logger.LogWarning($"Login failed with [username={request.Username}] in {GetType().Name}");
                return Unauthorized("Login failed");
            }
            
            _logger.LogInformation($"Login successfully with [username={request.Username}] in {GetType().Name}");
            HttpContext.Response.Cookies.Append("someInformation", token.Token);

            return Ok($"Login successfully, [token={token.Token}], [refreshToken={token.RefreshToken}]");
        }
        catch (OperationCanceledException)
        {
            message = $"Operation was cancelled in {DateTime.UtcNow}";
            _logger.LogInformation(message);

            return BadRequest(message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Exception occured while login with [request={request.Username}]: {ex.Message}");
            return BadRequest($"An exception occured in {GetType().Name}");
        }
    }

    [AllowAnonymous]
    [HttpPost("refresh")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RefreshTokenAsync(
        [FromBody] RefreshTokenRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _usersService.ValidateRefreshTokenAsync(
                request.Token, cancellationToken);
            if (result is null)
            {
                _logger.LogWarning($"Refresh token not received in {GetType().Name}");
                return Unauthorized("Access denied, unauthorized");
            }
            
            _logger.LogInformation($"Refresh token validated in {GetType().Name}");

            HttpContext.Response.Cookies.Append("someInformation", result.Token);
            
            return Ok($"Refresh token successfully generated [token={result.RefreshToken}]");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return BadRequest($"An exception occured in {GetType().Name}");
        }
    }

    [HttpGet("test")]
    public async Task<IActionResult> TestAccessController()
    {
        await Task.Delay(500);
        return Ok("access provided");
    }
}