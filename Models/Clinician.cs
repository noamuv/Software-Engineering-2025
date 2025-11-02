using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Software_Engineering_2025.Models
{
    public class Clinician
    {
        [Key]
        [ForeignKey("User")]
        public int User_ID { get; set; }  // PK/FK

        [StringLength(100)]
        public string? Department { get; set; }

        [StringLength(100)]
        public string? License_Number { get; set; }

        [StringLength(100)]
        public string? Specialisation { get; set; }

        // Navigation properties
        public virtual User? User { get; set; }
        public virtual ICollection<Patient>? Patients { get; set; }  // Clinician has many patients
    }
}