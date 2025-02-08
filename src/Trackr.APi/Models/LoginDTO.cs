using System.ComponentModel.DataAnnotations;

namespace Trackr.Api.Models
{
    public record LoginDTO
    {
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public required string Email { get; init; }
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters.")]
        [MaxLength(30, ErrorMessage = "Password must be maximum 30 characters.")]
        public required string Password { get; init; }
    }
}
