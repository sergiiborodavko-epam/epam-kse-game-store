using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using EpamKse.GameStore.Domain.Enums;

namespace EpamKse.GameStore.Domain.DTO.Role;

public class UpdateRoleDto
{
    [Required]
    public int? UserId { get; set; }
    
    [Required]
    public Roles? Role { get; set; }
}