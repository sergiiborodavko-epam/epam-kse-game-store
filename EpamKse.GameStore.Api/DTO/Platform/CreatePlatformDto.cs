using System.ComponentModel.DataAnnotations;

namespace EpamKse.GameStore.Api.DTO.Platform;

public class CreatePlatformDto
{
    [Required(AllowEmptyStrings = false, ErrorMessage = "Name is required")]
    public string Name { get; set; }
}