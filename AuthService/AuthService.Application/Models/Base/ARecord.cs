using System.Text.Json;
using System.Text.Json.Serialization;

namespace AuthService.Application.Models.Base;

public abstract record ARecord
{
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true,
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public sealed override string ToString()
    {
        return JsonSerializer.Serialize(this, GetType(), _jsonOptions);
    }
}