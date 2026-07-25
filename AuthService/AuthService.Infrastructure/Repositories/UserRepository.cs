using AuthService.Core.Entities;
using AuthService.Core.Interfaces;
using AuthService.Infrastructure.Repositories.Base;
using Microsoft.Extensions.Logging;

namespace AuthService.Infrastructure.Repositories;

public sealed class UserRepository(
    DatabaseContext context,
    ILogger<UserEntity> logger)
    : BaseRepository<UserEntity>(context, logger),
    IUserRepository
{
}