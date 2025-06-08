using System.ComponentModel.DataAnnotations;

namespace EpamKse.GameStore.Domain.DTO.Publisher;

public class UpdatePublisherDTO
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Name is required")]
    [MinLength(1, ErrorMessage = "Name cannot be empty")]
    [MaxLength(100, ErrorMessage = "Name can't be longer than 100 characters")]
    public string? Name { get; set; }

    [Url(ErrorMessage = "HomePageUrl must be a valid URL")]
    [MinLength(1, ErrorMessage = "HomePageUrl cannot be empty")]
    [MaxLength(500, ErrorMessage = "HomePageUrl can't be longer than 500 characters")]
    public string? HomePageUrl { get; set; }

    [MaxLength(2000, ErrorMessage = "Description can't be longer than 2000 characters")]
    public string? Description { get; set; }
}