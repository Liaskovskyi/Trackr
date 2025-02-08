using System.ComponentModel.DataAnnotations;

namespace Trackr.Api.Models
{
    public class RegisterDTO
    {
        [MinLength(3, ErrorMessage = "Username must be at least 3 characters.")]
        [MaxLength(20, ErrorMessage = "Username must be at most 20 characters.")]
        public required string Username { get; set; }
        public required string Password { get; set; }
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public required string Email { get; set; }
    }
}
