using AuthService.Application.Configs;
using AuthService.Core.Entities;
using AuthService.Core.Interfaces;
using Dapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;

namespace AuthService.Infrastructure.Repositories;

public sealed class UsersRepository : IUsersRepository
{
    private readonly ILogger<UsersRepository> _logger;
    
    private readonly NpgsqlConnection _connection;
    
    public UsersRepository(
        IOptions<DatabaseConfig> dbConfig,
        ILogger<UsersRepository> logger)
    {
        var connectionString = dbConfig.Value.CreateConnectionString();
        _logger = logger;
        _connection = new NpgsqlConnection(connectionString);
        
        _logger.LogInformation($"{GetType().Name} was initialized");
    }

    public async Task<long?> CreateUserAsync(
        User user,
        CancellationToken cancellationToken = default)
    {
        const string sql =
            "INSERT INTO users_tbl " +
            "(username, password_hash, role) " +
            "VALUES " +
            "(@username, @password_hash, @role) " +
            "RETURNING id;";
        
        var parameters = new DynamicParameters();
        parameters.Add("@username", user.Username);
        parameters.Add("@password_hash", user.PasswordHash);
        parameters.Add("@role", user.Role);
        
        var command = new CommandDefinition(
            sql,
            parameters,
            cancellationToken: cancellationToken);

        try
        {
            var createdUserId = await _connection.QuerySingleOrDefaultAsync<long?>(command);
            if (createdUserId is null)
            {
                _logger.LogWarning($"User creation failed [user={user}] in {GetType().Name}");
                return null;
            }
            
            _logger.LogInformation($"User successfully created with [id={createdUserId}] in {GetType().Name}");
            return createdUserId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    public async Task<User?> GetByUsernameAsync(
        string username,
        CancellationToken cancellationToken = default)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@username", username);

        const string sql =
            "SELECT " +
                "id AS Id, " +
                "username AS Username, " +
                "password_hash AS PasswordHash, " +
                "role AS Role " +
            "FROM public.users_tbl " +
            "WHERE username = @username;";
        
        var command = new CommandDefinition(
            sql,
            parameters,
            cancellationToken: cancellationToken);

        try
        {
            var user = await _connection.QuerySingleOrDefaultAsync<User>(command);
            if (user is null)
            {
                _logger.LogWarning($"User not found by [username={username}] in {GetType().Name}");
                return null;
            }
            
            _logger.LogInformation($"User successfully found by [username={user.Username}] in {GetType().Name}");
            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }
    
    public async Task<User?> GetByIdAsync(
        long userId,
        CancellationToken cancellationToken = default)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@id", userId);

        const string sql =
            "SELECT " +
            "id AS Id, " +
            "username AS Username, " +
            "password_hash AS PasswordHash, " +
            "role AS Role " +
            "FROM public.users_tbl " +
            "WHERE id = @id;";
        
        var command = new CommandDefinition(
            sql,
            parameters,
            cancellationToken: cancellationToken);

        try
        {
            var user = await _connection.QuerySingleOrDefaultAsync<User>(command);
            if (user is null)
            {
                _logger.LogWarning($"User not found by [id={userId}] in {GetType().Name}");
                return null;
            }
            
            _logger.LogInformation($"User successfully found by [id={user.Id}] in {GetType().Name}");
            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }
}