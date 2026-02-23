using AuthService.Application.Configs;
using AuthService.Core.Entities;
using AuthService.Core.Interfaces;
using Dapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;

namespace AuthService.Infrastructure.Repositories;

public class AuthRepository : IAuthRepository
{
    private readonly ILogger<AuthRepository> _logger;
    
    private readonly NpgsqlConnection _connection;
    
    public AuthRepository(
        IOptions<DatabaseConfig> dbConfig,
        ILogger<AuthRepository> logger)
    {
        var connectionString = dbConfig.Value.CreateConnectionString();
        _logger = logger;
        _connection = new NpgsqlConnection(connectionString);
        
        _logger.LogInformation($"{GetType().Name} was initialized");
    }

    public async Task<bool> AddRefreshTokenAsync(
        RefreshToken refreshToken,
        CancellationToken cancellationToken = default)
    {
        const string sql =
            "INSERT INTO public.refresh_tokens_tbl " +
            "(user_id, token, expires_at) " +
            "VALUES " +
            "(@user_id, @token, @expires_at);";

        var parameters = new DynamicParameters();
        parameters.Add("@user_id", refreshToken.UserId);
        parameters.Add("@token", refreshToken.Token);
        parameters.Add("@expires_at", refreshToken.ExpiresAt);
        
        var command = new CommandDefinition(
            sql, 
            parameters,
            cancellationToken: cancellationToken);

        try
        {
            var rowsAffected = await _connection.ExecuteAsync(command);
            if (rowsAffected == 0)
            {
                _logger.LogWarning($"Refresh token creation failed in {GetType().Name}");
                return false;
            }
            
            _logger.LogInformation($"Refresh token successfully created in {GetType().Name}");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    public async Task<RefreshToken?> GetByTokenAsync(
        string token,
        CancellationToken cancellationToken = default)
    {
        const string sql =
            "SELECT " +
                "id AS Id, " +
                "user_id AS UserId, " +
                "token AS Token, " +
                "expires_at AS ExpiresAt " +
            "FROM public.refresh_tokens_tbl " +
            "WHERE token = @token;";

        var parameters = new DynamicParameters();
        parameters.Add("@token", token);
        
        var command = new CommandDefinition(
            sql, 
            parameters,
            cancellationToken: cancellationToken);

        try
        {
            var refreshToken = await _connection.QuerySingleOrDefaultAsync<RefreshToken>(command);
            if (refreshToken is null)
            {
                _logger.LogWarning($"Refresh token not found in {GetType().Name}");
                return null;
            }
            
            _logger.LogInformation($"Refresh token successfully found in {GetType().Name}");
            return refreshToken;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    public async Task<RefreshToken?> GetByUserIdAsync(
        long userId,
        CancellationToken cancellationToken = default)
    {
        const string sql =
            "SELECT " +
                "id AS Id, " +
                "user_id AS UserId, " +
                "token AS Token, " +
                "expires_at AS ExpiresAt " +
            "FROM public.refresh_tokens_tbl " +
            "WHERE user_id = @user_id;";
        
        var parameters = new DynamicParameters();
        parameters.Add("@user_id", userId);
        
        var command = new CommandDefinition(
            sql, 
            parameters,
            cancellationToken: cancellationToken);

        try
        {
            var refreshToken = await _connection.QuerySingleOrDefaultAsync<RefreshToken>(command);
            if (refreshToken is null)
            {
                _logger.LogWarning($"Refresh token not found in {GetType().Name}");
                return null;
            }
            
            _logger.LogInformation($"Refresh token successfully found in {GetType().Name}");
            return refreshToken;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    public async Task<bool> DeleteRefreshTokenAsync(
        string token, 
        CancellationToken cancellationToken = default)
    {
        const string sql =
            "DELETE FROM public.refresh_tokens_tbl " +
            "WHERE token = @token;";
        
        var parameters = new DynamicParameters();
        parameters.Add("@token", token);
        
        var command = new CommandDefinition(
            sql, 
            parameters,
            cancellationToken: cancellationToken);

        try
        {
            var rowsAffected = await _connection.ExecuteAsync(command);
            if (rowsAffected == 0)
            {
                _logger.LogWarning($"Refresh token revoke failed in {GetType().Name}");
                return false;
            }
            
            _logger.LogInformation($"Refresh token successfully revoked in {GetType().Name}");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }
}