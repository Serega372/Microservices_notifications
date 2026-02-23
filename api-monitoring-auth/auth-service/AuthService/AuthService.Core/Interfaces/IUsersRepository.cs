using AuthService.Core.Entities;

namespace AuthService.Core.Interfaces;

public interface IUsersRepository
{
    Task<long?> CreateUserAsync(User user, CancellationToken cancellationToken = default);
    
    Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
    
    Task<User?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
}