using System.ComponentModel.DataAnnotations;

namespace EpamKse.GameStore.Domain.DTO.License;

public class VerifyLicenseDto
{
    [Required]
    public string Key { get; set; }
}