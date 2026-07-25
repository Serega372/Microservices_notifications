using AuthService.Core.Entities.Base;
using System.Text.Json.Serialization;

namespace AuthService.Core.Entities;

public sealed class PermissionEntity : AEntity
{
    [JsonIgnore]
    public ICollection<RolePermissionEntity> RolesPermissions { get; set; } = [];
}