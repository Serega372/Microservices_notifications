using Microsoft.AspNetCore.Mvc;
using NotificationsService.Application.Interfaces;
using NotificationsService.Application.Models;
using System.Security.Claims;

namespace NotificationsService.WebApi.Controllers;

[ApiController]
[Route("api/notifications")]
public class NotificationsController : ControllerBase
{
    private readonly ILogger<NotificationsController> _logger;

    private readonly INotificationsService _notificationsService;

    public NotificationsController(
        ILogger<NotificationsController> logger,
        INotificationsService notificationsService)
    {
        _logger = logger;
        _notificationsService = notificationsService;

        _logger.LogInformation($"{GetType().Name} was initialized");
    }

    [HttpPost]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SendMessageAsync(
        [FromBody] SendMessageRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (request.Id.HasValue)
            {
                await _notificationsService.ProcessRetryMessageAsync(request, cancellationToken);
            }

            var userId = long.Parse(HttpContext.User.FindFirstValue("Id")
                ?? throw new Exception("User not found"));
            var messageWithUserIdRequest = new SendMessageWithUserIdRequest
            {
                Type = request.Type,
                Payload = request.Payload,
                UserId = userId
            };

            await _notificationsService.ProcessMessageAsync(messageWithUserIdRequest, cancellationToken);

            return Ok();
        }
        catch (OperationCanceledException)
        {
            var message = $"Operation was cancelled in {DateTime.UtcNow}";
            _logger.LogInformation(message);
            return BadRequest(message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Exception occured while send message: {ex.Message}");
            return BadRequest($"An exception occured in {GetType().Name}");
        }
    }
}