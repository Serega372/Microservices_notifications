using AuthService.Application.Interfaces;
using AuthService.Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.WebApi.Controllers;

[ApiController]
[Route("api/admin")]
public class AdminController : ControllerBase
{
    private readonly ILogger<AdminController> _logger;
    
    private readonly IUsersService _usersService;

    public AdminController(
        ILogger<AdminController> logger,
        IUsersService usersService)
    {
        _logger = logger;
        _usersService = usersService;
        
        _logger.LogInformation($"{GetType().Name} was initialized");
    }
    
    [Authorize(Roles = "Admin")]
    [HttpPost("register")]
    [ProducesResponseType(typeof(long), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RegisterUserAsAdminAsync(
        [FromBody] RegisterUserRequest request,
        CancellationToken cancellationToken = default)
    {
        var message = string.Empty;
        
        try
        {
            var createdUserId = await _usersService.CreateUserAsync(
                request, 
                true,
                cancellationToken);
            
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
}