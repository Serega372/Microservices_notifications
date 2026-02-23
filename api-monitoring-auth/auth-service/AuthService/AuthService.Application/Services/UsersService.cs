using AuthService.Application.Interfaces;
using AuthService.Application.Models;
using AuthService.Core.Entities;
using AuthService.Core.Enums;
using AuthService.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace AuthService.Application.Services;

public sealed class UsersService : IUsersService
{
    private readonly ILogger<UsersService> _logger;
    
    private readonly IUsersRepository _usersRepository;
    
    private readonly IAuthRepository _authRepository;
    
    private readonly IPasswordHasherService _passwordHasherService;
    
    private readonly IJwtProviderService _jwtProviderService;

    public UsersService(
        ILogger<UsersService> logger,
        IPasswordHasherService passwordHasherService,
        IAuthRepository authRepository,
        IJwtProviderService jwtProviderService,
        IUsersRepository usersRepository)
    {
        _logger = logger;
        _usersRepository = usersRepository;
        _authRepository = authRepository;
        _passwordHasherService = passwordHasherService;
        _jwtProviderService = jwtProviderService;
        
        _logger.LogInformation($"{GetType().Name} was initialized");
    }

    public async Task<long?> CreateUserAsync(
        RegisterUserRequest request,
        bool isAdmin = false,
        CancellationToken cancellationToken = default)
    {
        if (!isAdmin && request.Role != null)
            throw new Exception("You not admin!");
        
        var hashedPassword = _passwordHasherService.GeneratePasswordHash(request.Password);
        var role = isAdmin ? request.Role ?? UserRole.User : UserRole.User;
        var userEntity = new User
        {
            PasswordHash = hashedPassword,
            Username = request.Username,
            Role = role
        };

        var createdUserId = await _usersRepository.CreateUserAsync(userEntity, cancellationToken);
        if (createdUserId is null)
        {
            _logger.LogWarning($"User not created with {request.Username} in {GetType().Name}");
            return null;
        }
        
        _logger.LogInformation($"User successfully created with [id={createdUserId}] in {GetType().Name}]");
        return createdUserId;
    }

    public async Task<LoginUserResponse?> LoginAsync(
        LoginUserRequest request,
        CancellationToken cancellationToken = default)
    {
        var userEntity = await _usersRepository.GetByUsernameAsync(request.Username, cancellationToken);
        if (userEntity is null)
        {
            _logger.LogWarning($"User not found by [username={request.Username}] in {GetType().Name}");
            return null;
        }

        var isVerified = _passwordHasherService.VerifyPassword(request.Password, userEntity.PasswordHash);
        if (!isVerified)
        {
            _logger.LogWarning($"User with [username={userEntity.Username}] password verification failed in {GetType().Name}");
            return null;
        }
        
        _logger.LogInformation($"User with [username={userEntity.Username}] password verified in {GetType().Name}");
        
        var jwtToken = _jwtProviderService.GenerateToken(userEntity);
        var jwtRefreshToken = await _jwtProviderService.GenerateRefreshToken(userEntity.Id, cancellationToken);
        if (jwtRefreshToken is null)
        {
            _logger.LogWarning($"Refresh token generation failed in {GetType().Name}");
            return null;
        }
        
        _logger.LogInformation($"Refresh token successfully generated in {GetType().Name}");
        
        var response = new LoginUserResponse
        {
            Token = jwtToken,
            RefreshToken = jwtRefreshToken
        };
        
        return response;
    }
    
    public async Task<RefreshTokenResponse?> ValidateRefreshTokenAsync(
        string token,
        CancellationToken cancellationToken = default)
    {
        var refreshToken = await _authRepository.GetByTokenAsync(token, cancellationToken);
        if (refreshToken?.Token is null || refreshToken.ExpiresAt < DateTime.UtcNow)
            return null;

        await _authRepository.DeleteRefreshTokenAsync(refreshToken.Token, cancellationToken);

        var user = await _usersRepository.GetByIdAsync(refreshToken.UserId, cancellationToken);
        if (user is null)
        {
            _logger.LogWarning($"User not found by [id={refreshToken.UserId}] in {GetType().Name}");
            return null;
        }

        var newToken = _jwtProviderService.GenerateToken(user);
        var newRefreshToken = await _jwtProviderService.GenerateRefreshToken(
            user.Id, cancellationToken);
        if (newRefreshToken is null)
        {
            _logger.LogWarning($"Refresh token generation failed in {GetType().Name}");
            return null;
        }
        
        _logger.LogInformation($"Refresh token successfully generated in {GetType().Name}");

        var response = new RefreshTokenResponse
        {
            Token = newToken,
            RefreshToken = newRefreshToken,
        };
        
        return response;
    }
}