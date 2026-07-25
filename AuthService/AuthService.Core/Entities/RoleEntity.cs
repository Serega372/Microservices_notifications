using AuthService.Core.Entities.Base;
using System.Text.Json.Serialization;

namespace AuthService.Core.Entities;

public sealed class RoleEntity : AEntity
{
    [JsonIgnore]
    public ICollection<UserEntity> Users { get; set; } = [];

    [JsonIgnore]
    public ICollection<RolePermissionEntity> RolesPermissions { get; set; } = [];
}