using AuthService.Application.Models.Base;
using System.ComponentModel.DataAnnotations;

namespace AuthService.Application.Models.Auth;

public sealed record TokenVerifyRequest : ARecord
{
    [Required]
    public required Guid AccessToken { get; init; }
}