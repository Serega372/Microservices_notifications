using AuthService.Core.Entities.Base;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AuthService.Core.Entities;

public sealed class RolePermissionEntity : AEntity
{
    [Required]
    public required long RoleId { get; set; }

    [Required]
    public required long PermissionId { get; set; }

    [JsonIgnore]
    public RoleEntity? Role { get; set; }

    [JsonIgnore]
    public PermissionEntity? Permission { get; set; }
}