using System.ComponentModel.DataAnnotations;

namespace Software_Engineering_2025.DTOs
{
    public class UserCreateRequest
    {
        [Required]
        public int User_Type_ID { get; set; }

        [StringLength(50)]
        public string? Title { get; set; }

        [Required]
        [StringLength(100)]
        public string First_Name { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Last_Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(255)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; } = string.Empty;
    }
}