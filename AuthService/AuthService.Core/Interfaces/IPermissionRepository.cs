using AuthService.Core.Entities;

namespace AuthService.Core.Interfaces;

public interface IPermissionRepository : IBaseRepository<PermissionEntity>
{
    Task<IEnumerable<string>> GetByRoleIdAsync(long roleId, bool asNoTracking = true, CancellationToken cancellationToken = default);
}