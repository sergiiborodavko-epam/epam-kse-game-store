using System.ComponentModel.DataAnnotations;

namespace EpamKse.GameStore.Domain.DTO.Platform;

public class UpdatePlatformDto
{
    
    [MinLength(1, ErrorMessage = "Name cannot be empty")]
    public string? Name { get; set; }
}