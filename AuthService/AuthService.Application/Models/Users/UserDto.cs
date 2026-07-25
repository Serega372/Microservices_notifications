using AuthService.Application.Models.Base;
using System.ComponentModel.DataAnnotations;

namespace AuthService.Application.Models.Users;

public sealed record UserDto : ARecord
{
    [Required]
    public required long Id { get; init; }

    [Required]
    public required string Name { get; init; }

    [Required]
    public required string Username { get; init; }

    [Required]
    public required string Email { get; init; }

    [Required]
    public required long RoleId { get; init; }
}