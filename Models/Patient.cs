using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Software_Engineering_2025.Models
{
    public class Patient
    {
        [Key]
        [ForeignKey("User")]
        public int User_ID { get; set; }  // PK/FK

        [ForeignKey("Clinician")]
        public int? Clinician_User_ID { get; set; }  // FK to assigned clinician

        [ForeignKey("Gender")]
        public int? Gender_ID { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        public DateTime? Date_of_Birth { get; set; }

        // Navigation properties
        public virtual User? User { get; set; }
        public virtual Clinician? Clinician { get; set; }
        public virtual Gender? Gender { get; set; }
    }
}