using AuthService.Application.Models.Base;
using System.ComponentModel.DataAnnotations;

namespace AuthService.Application.Models.Auth;

public sealed record LoginResponse : ARecord
{
    [Required]
    public required Guid AccessToken { get; init; }

    [Required]
    public required Guid RefreshToken { get; init; }
}