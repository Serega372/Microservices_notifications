using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NotificationsService.Core.Entities.Base;

public abstract class AEntity
{
    [Required]
    public long Id { get; set; }

    [Required]
    public DateTime CreationDate { get; set; }

    [Required]
    public DateTime LastUpdate { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime? DeletedAt { get; set; }

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true,
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public override string ToString()
    {
        return JsonSerializer.Serialize(this, GetType(), _jsonOptions);
    }
}