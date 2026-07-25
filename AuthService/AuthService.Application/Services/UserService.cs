using AuthService.Application.Interfaces;
using AuthService.Application.Models.Users;
using AuthService.Core.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace AuthService.Application.Services;

public sealed class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    private readonly ILogger<UserService> _logger;

    private readonly IMapper _mapper;

    public UserService(
        IUserRepository userRepository,
        ILogger<UserService> logger,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _logger = logger;
        _mapper = mapper;

        _logger.LogInformation($"{GetType().Name} was initialized");
    }
}