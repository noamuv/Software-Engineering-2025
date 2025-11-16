using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Software_Engineering_2025.Models
{
    public class CarerAccess
    {
        [Key]
        public int Access_ID { get; set; }

        [Required]
        [ForeignKey("CarerUser")]
        public int Carer_User_ID { get; set; }

        [Required]
        [ForeignKey("PatientUser")]
        public int Patient_User_ID { get; set; }

        [Required]
        [StringLength(50)]
        public string Permission_Level { get; set; }  // "Full Access", "View Only", "Emergency Only"

        [Required]
        [ForeignKey("GrantedByUser")]
        public int Granted_By_User_ID { get; set; }  // Who granted this access (usually the Patient)

        [ForeignKey("RevokedByUser")]
        public int? Revoked_By_User_ID { get; set; }  // Who revoked it (if revoked)

        public DateTime Granted_At { get; set; } = DateTime.Now;

        public DateTime? Revoked_At { get; set; }

        // Navigation properties
        public virtual User? CarerUser { get; set; }
        public virtual User? PatientUser { get; set; }
        public virtual User? GrantedByUser { get; set; }
        public virtual User? RevokedByUser { get; set; }
    }
}