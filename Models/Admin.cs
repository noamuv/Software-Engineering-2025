using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Software_Engineering_2025.Models
{
    public class Admin
    {
        [Key]
        [ForeignKey("User")]
        public int User_ID { get; set; }  // PK/FK 

        [StringLength(100)]
        public string? Admin_Level { get; set; }  // Indicates Admin hierarchy - "Super Admin", "Admin"

        public DateTime? Last_Login { get; set; }

        // Navigation property
        public virtual User? User { get; set; }  // Links back to User table
    }
}
