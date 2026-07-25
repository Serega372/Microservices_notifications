namespace AuthService.Application.Interfaces;

public interface IPasswordHasherService
{
    public string GeneratePasswordHash(string password);

    public bool VerifyPassword(string password, string hashedPassword);
}