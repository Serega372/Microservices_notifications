using AuthService.Application.Models.RefreshTokens;
using AuthService.Core.Entities;
using AutoMapper;

namespace AuthService.Application.Mapping;

public sealed class RefreshTokenMappingProfile : Profile
{
    public RefreshTokenMappingProfile()
    {
        CreateMap<RefreshTokenEntity, RefreshTokenDto>();
    }
}