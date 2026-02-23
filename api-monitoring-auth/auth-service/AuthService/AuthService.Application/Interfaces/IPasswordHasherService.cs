namespace AuthService.Application.Interfaces;

public interface IPasswordHasherService
{
    string GeneratePasswordHash(string password);
    
    bool VerifyPassword(string password, string hashedPassword);
}