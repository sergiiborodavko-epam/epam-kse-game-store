using System.ComponentModel.DataAnnotations;

namespace EpamKse.GameStore.Api.DTO.Auth;

public class LoginDTO
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    [MinLength(5, ErrorMessage = "Email must be at least 5 characters.")]
    [MaxLength(30, ErrorMessage = "Email can't be longer than 30 characters.")]
    public string Email { set; get; }
    
    
    [Required(ErrorMessage = "Password is required.")]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters.")]
    [MaxLength(30, ErrorMessage = "Password can't be longer than 30 characters.")]
    public string Password { set; get; }
}