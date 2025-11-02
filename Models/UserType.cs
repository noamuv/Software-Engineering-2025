using System.ComponentModel.DataAnnotations;

namespace Software_Engineering_2025.Models
{
    public class UserType
    {
        [Key]
        public int User_Type_ID { get; set; }

        [Required]
        [StringLength(50)]
        public string Type_Name { get; set; }  // "Admin", "Patient", "Clinician", "Carer"

        [StringLength(255)]
        public string? Description { get; set; } // Description of the Users role

        // Navigation property - one UserType has many Users
        public virtual ICollection<User>? Users { get; set; }
    }
}