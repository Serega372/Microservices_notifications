using AuthService.Core.Entities;
using AuthService.Core.Entities.Base;
using AuthService.Core.Interfaces;
using AuthService.Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AuthService.Infrastructure.Repositories;

public sealed class PermissionRepository(
    DatabaseContext context,
    ILogger<PermissionRepository> logger) 
    : BaseRepository<PermissionEntity>(context, logger),
    IPermissionRepository
{
    public async Task<IEnumerable<string>> GetByRoleIdAsync(long roleId, bool asNoTracking = true, CancellationToken cancellationToken = default)
    {
        var query = asNoTracking
           ? DbSet.AsNoTracking()
           : DbSet;

        var result = query
            .Where(entity => entity.RolesPermissions
                .Any(rolePermission => rolePermission.RoleId == roleId))
            .Select(entity => entity.Name);

        return await result.ToListAsync(cancellationToken);
    }
}