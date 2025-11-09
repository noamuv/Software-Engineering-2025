using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Software_Engineering_2025.Models
{
    public class User
    {
        [Key]
        public int User_ID { get; set; }

        [Required]
        [ForeignKey("UserType")]
        public int User_Type_ID { get; set; }  // FK to UserType table 

        [StringLength(20)]
        public string? Title { get; set; }  // "Mr", "Mrs", "Dr" 

        [Required]
        [StringLength(100)]
        public string First_Name { get; set; }

        [Required]
        [StringLength(100)]
        public string Last_Name { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(255)]
        public string Email { get; set; } 

        [Required]
        public string Password_Hash { get; set; }  // Store hashed password for security and encryption

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Active";  // Active, Disabled, Pending

        public DateTime Created_At { get; set; } = DateTime.Now;

        public bool Is_Activated { get; set; } = false;  // Defines if account is activated via Yes or No

        [StringLength(100)]
        public string? Activation_Code { get; set; }

        // Navigation properties
        public virtual UserType? UserType { get; set; }  // Links to UserType table
    }
}