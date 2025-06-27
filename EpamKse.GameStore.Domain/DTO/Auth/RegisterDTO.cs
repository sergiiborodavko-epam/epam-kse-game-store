namespace EpamKse.GameStore.Domain.DTO.Auth;

using System.ComponentModel.DataAnnotations;

public class RegisterDTO {
    [Required(ErrorMessage = "UserName is required.")]
    [MinLength(3, ErrorMessage = "UserName must be at least 3 characters.")]
    [MaxLength(30, ErrorMessage = "UserName can't be longer than 30 characters.")]
    public string UserName { get; set; }
    
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    [MinLength(5, ErrorMessage = "Email must be at least 5 characters.")]
    [MaxLength(30, ErrorMessage = "Email can't be longer than 30 characters.")]
    public string Email { get; set; }
    
    [Required(ErrorMessage = "Password is required.")]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters.")]
    [MaxLength(30, ErrorMessage = "Password can't be longer than 30 characters.")]
    public string Password { get; set; }
    
    [Required(ErrorMessage = "FullName is required.")]
    [MinLength(3, ErrorMessage = "FullName must be at least 3 characters.")]
    [MaxLength(30, ErrorMessage = "FullName can't be longer than 30 characters.")]
    public string FullName { get; set; }
    
    [Required(ErrorMessage = "Country is required.")]
    [MinLength(2, ErrorMessage = "Country must be at least 2 characters")]
    [MaxLength(2, ErrorMessage = "Country must be exactly 2 characters")]
    public string Country { get; set; }
}
