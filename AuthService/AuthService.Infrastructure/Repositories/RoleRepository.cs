using AuthService.Core.Entities;
using AuthService.Core.Interfaces;
using AuthService.Infrastructure.Repositories.Base;
using Microsoft.Extensions.Logging;

namespace AuthService.Infrastructure.Repositories;

public sealed class RoleRepository(
    DatabaseContext context,
    ILogger<RoleRepository> logger) 
    : BaseRepository<RoleEntity>(context, logger),
    IRoleRepository
{
}