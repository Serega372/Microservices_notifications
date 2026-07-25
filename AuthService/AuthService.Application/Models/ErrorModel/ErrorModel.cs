using AuthService.Application.Models.Base;
using System.ComponentModel.DataAnnotations;

namespace AuthService.Application.Models.ErrorModel;

public sealed record ErrorModel(
    string message, 
    string? details = null) 
    : ARecord
{
}