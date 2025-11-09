using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Software_Engineering_2025.Models
{
    public class Carer
    {
        // Primary Key AND Foreign Key to User
        [Key]
        [ForeignKey("User")]
        public int User_ID { get; set; }

        [StringLength(500)]
        public string? Availability_Schedule { get; set; }  // When the carer is available

        // Navigation properties
        public virtual User? User { get; set; }
        public virtual ICollection<CarerAccess>? CarerAccesses { get; set; }  // Patients this carer can access
    }
}