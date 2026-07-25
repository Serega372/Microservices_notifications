using AuthService.Core.Entities;
using AuthService.Core.Interfaces;
using AuthService.Infrastructure.Repositories.Base;
using Microsoft.Extensions.Logging;

namespace AuthService.Infrastructure.Repositories;

public sealed class RolePermissionRepository(
    DatabaseContext context,
    ILogger<RolePermissionRepository> logger) 
    : BaseRepository<RolePermissionEntity>(context, logger),
    IRolePermissionRepository
{
}