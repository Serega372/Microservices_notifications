using AuthService.Application.Interfaces;
using AuthService.Application.Models.Auth;
using AuthService.Application.Models.ErrorModel;
using AuthService.Core;
using AuthService.Core.Entities;
using AuthService.Core.Exceptions;
using AuthService.Core.Interfaces;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace AuthService.Application.Services;

public sealed class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;

    private readonly IRoleRepository _roleRepository;

    private readonly IAccessTokenRepository _accessTokenRepository;

    private readonly IRefreshTokenRepository _refreshTokenRepository;

    private readonly IPasswordHasherService _passwordHasherService;

    private readonly IUnitOfWork _unitOfWork;

    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IAccessTokenRepository accessTokenRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IPasswordHasherService passwordHasherService,
        IUnitOfWork unitOfWork,
        ILogger<AuthService> logger)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _accessTokenRepository = accessTokenRepository;
        _passwordHasherService = passwordHasherService;
        _unitOfWork = unitOfWork;
        _logger = logger;

        _logger.LogInformation($"{GetType().Name} was initialized");
    }

    public async Task<(RegisterResponse? response, ErrorModel? errorModel)> RegisterAsync(RegisterRequest request, bool isAdmin = false, CancellationToken cancellationToken = default)
    {
        await using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            Expression<Func<UserEntity, bool>> userByEmailFilter = entity => entity.Email == request.Email;
            var existingUser = (await _userRepository.GetAsync(userByEmailFilter, cancellationToken: cancellationToken)).FirstOrDefault();
            if (existingUser is not null)
            {
                var message = $"[{nameof(UserEntity)}] with email [{existingUser.Email}] already exists";
                _logger.LogWarning(message);
                return (null, new ErrorModel(message));
            }

            var allRoles = await _roleRepository.GetAsync(cancellationToken: cancellationToken);
            var defaultUserRole = allRoles.FirstOrDefault(role => role.Name == ApplicationConstants.DefaultUserRole);
            if (defaultUserRole is null)
            {
                var message = $"[{nameof(RoleEntity)}] with name [{ApplicationConstants.DefaultUserRole}] not found";
                _logger.LogCritical(message);
                throw new RoleNotFoundException(message);
            }

            var handleRoleId = request.RoleId ?? defaultUserRole.Id;
            var hashedPassword = _passwordHasherService.GeneratePasswordHash(request.Password);
            var userEntityToCreate = new UserEntity
            {
                Id = default,
                Name = request.Name,
                Username = request.Username,
                Email = request.Email,
                PasswordHash = hashedPassword,
                RoleId = isAdmin
                    ? handleRoleId
                    : defaultUserRole.Id
            };

            var createdUser = await _userRepository.AddAsync(userEntityToCreate, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var refreshTokenDto = await _refreshTokenRepository.CreateAsync(createdUser.Id, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var accessTokenDto = await _accessTokenRepository.CreateAsync(createdUser.Id, refreshTokenDto.Id, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(transaction, cancellationToken);

            return (new RegisterResponse
            {
                AccessToken = accessTokenDto.Token,
                RefreshToken = refreshTokenDto.Token
            }, null);
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, $"Exception occured while register user with email [{request.Email}]");
            await _unitOfWork.RollbackTransactionAsync(transaction, cancellationToken);
            throw;
        }
    }

    public async Task<(LoginResponse? response, ErrorModel? errorModel)> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        await using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            Expression<Func<UserEntity, bool>> userByEmailFilter = entity => entity.Email == request.Email;
            var existingUser = (await _userRepository.GetAsync(userByEmailFilter, cancellationToken: cancellationToken)).FirstOrDefault();
            if (existingUser is null)
            {
                var message = $"[{nameof(UserEntity)}] not found by email [{request.Email}]";
                _logger.LogWarning(message);
                return (null, new ErrorModel(message));
            }

            var isHashesEquals = _passwordHasherService.VerifyPassword(request.Password, existingUser.PasswordHash);
            if (!isHashesEquals)
            {
                var message = $"Invalid password for email [{existingUser.Email}]";
                _logger.LogWarning(message);
                return (null, new ErrorModel(message));
            }

            var refreshTokenDto = await _refreshTokenRepository.CreateAsync(existingUser.Id, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var accessTokenDto = await _accessTokenRepository.CreateAsync(existingUser.Id, refreshTokenDto.Id, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(transaction, cancellationToken);

            return (new LoginResponse
            {
                AccessToken = accessTokenDto.Token,
                RefreshToken = refreshTokenDto.Token
            }, null);
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, $"Exception occured while login user with email [{request.Email}]");
            await _unitOfWork.RollbackTransactionAsync(transaction, cancellationToken);
            throw;
        }
    }

    public async Task<ErrorModel?> Logout(LogoutRequest request, CancellationToken cancellationToken = default)
    {
        var foundRefreshToken = await _refreshTokenRepository.GetWithAccessTokensAsync(request.RefreshToken, true, cancellationToken: cancellationToken);
        if (foundRefreshToken is null)
        {
            _logger.LogWarning($"Active [{nameof(RefreshTokenEntity)}] [{request.RefreshToken}] not found");
            return new ErrorModel($"[{nameof(RefreshTokenEntity)}] not active, maybe already logout");
        }

        foundRefreshToken.RevocationDate = DateTime.UtcNow;
        foreach (var accessToken in foundRefreshToken.AccessTokens)
        {
            accessToken.RevocationDate = DateTime.UtcNow;
        }

        _refreshTokenRepository.Update(foundRefreshToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return null;
    }
}