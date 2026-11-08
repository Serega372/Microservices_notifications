namespace NotificationsService.Application.Configs;

public sealed class AuthServiceOptions
{
    public required string BaseUrl { get; init; }

    public required string VerifyPath { get; init; }
}