using System.ComponentModel.DataAnnotations;

namespace Software_Engineering_2025.DTOs
{
    public class UserUpdateRequest
    {
        [StringLength(50)]
        public string? Title { get; set; }

        [StringLength(100)]
        public string? First_Name { get; set; }

        [StringLength(100)]
        public string? Last_Name { get; set; }

        [EmailAddress]
        [StringLength(255)]
        public string? Email { get; set; }

        public int? User_Type_ID { get; set; }
    }
}