using AuthService.Application.Models.Base;
using System.ComponentModel.DataAnnotations;

namespace AuthService.Application.Models.Auth;

public sealed record TokenRefreshRequest : ARecord
{
    [Required]
    public required Guid RefreshToken { get; init; }
}