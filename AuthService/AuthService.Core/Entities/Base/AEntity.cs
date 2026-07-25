using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AuthService.Core.Entities.Base;

public abstract class AEntity
{
    [Required]
    public required long Id { get; set; }

    [Required]
    public required string Name { get; set; }
    
    public string? Description { get; set; }

    public DateTime CreationDate { get; set; }

    public DateTime LastUpdate { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime? DeletedAt { get; set; }

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true,
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
        PropertyNamingPolicy = JsonNamingPolicy.KebabCaseLower
    };

    public override string ToString()
    {
        return JsonSerializer.Serialize(this, GetType(), _jsonOptions);
    }
}