using AuthService.Application.Interfaces;

namespace AuthService.Infrastructure.AuthHelpers;

public sealed class PasswordHasherService : IPasswordHasherService
{
    public string GeneratePasswordHash(string password) => BCrypt.Net.BCrypt.EnhancedHashPassword(password);
    
    public bool VerifyPassword(string password, string hashedPassword) => 
        BCrypt.Net.BCrypt.EnhancedVerify(password, hashedPassword);
}