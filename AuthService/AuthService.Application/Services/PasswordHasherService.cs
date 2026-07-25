using AuthService.Application.Configs;
using AuthService.Application.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;

namespace AuthService.Application.Services;

public sealed class PasswordHasherService : IPasswordHasherService
{
    private readonly ILogger<PasswordHasherService> _logger;

    private readonly PasswordHasherConfig _config;

    public PasswordHasherService(
        IOptions<PasswordHasherConfig> config,
        ILogger<PasswordHasherService> logger)
    {
        _config = config.Value;
        _logger = logger;

        _logger.LogInformation($"{GetType().Name} was initialized");
    }

    public string GeneratePasswordHash(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(_config.SaltBytes);
        var hash = Rfc2898DeriveBytes.Pbkdf2(Encoding.UTF8.GetBytes(password), salt, _config.Iterations, HashAlgorithmName.SHA256, 
            _config.OutputHashLength);

        return $"{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
    }

    public bool VerifyPassword(string password, string hashedPassword)
    {
        var hashedPasswordParts = hashedPassword.Split('.');
        if (hashedPasswordParts.Length != 2)
        {
            _logger.LogCritical($"Password must contains only 2 parts - salt and hash, but found [{hashedPassword}]");
            throw new InvalidOperationException();
        }

        var existingSalt = Convert.FromBase64String(hashedPasswordParts[0]);
        var existingPasswordHash = Convert.FromBase64String(hashedPasswordParts[1]);
        var hashToCompare = Rfc2898DeriveBytes.Pbkdf2(Encoding.UTF8.GetBytes(password), existingSalt, _config.Iterations, HashAlgorithmName.SHA256,
            _config.OutputHashLength);

        return CryptographicOperations.FixedTimeEquals(hashToCompare, existingPasswordHash);
    }
}