using AuthService.Application.Models.Base;
using System.ComponentModel.DataAnnotations;

namespace AuthService.Application.Models.Auth;

public sealed record RegisterRequest : ARecord
{
    [Required]
    public required string Name { get; set; }

    [Required]
    public required string Username { get; set; }

    [Required]
    public required string Email { get; set; }

    [Required]
    public required string Password { get; set; }

    public long? RoleId { get; set; }
}