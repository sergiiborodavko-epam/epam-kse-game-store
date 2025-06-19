using System.ComponentModel.DataAnnotations;

namespace EpamKse.GameStore.Domain.DTO.License;

public class CreateLicenseDto
{
    [Required]
    public int? OrderId { get; set; }
}