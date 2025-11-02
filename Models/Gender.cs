using System.ComponentModel.DataAnnotations;

namespace Software_Engineering_2025.Models
{
    public class Gender
    {
        [Key]
        public int Gender_ID { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }  // "Male", "Female", "Non-Binary", "Prefer not to say"

        // Navigation property
        public virtual ICollection<Patient>? Patients { get; set; }
    }
}