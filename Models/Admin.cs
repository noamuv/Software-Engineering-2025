using System;
using System.ComponentModel.DataAnnoations;

namespace Software_Engineering_2025.Models

{
    public class Admin
    {
        [Key]
        public int Admin_ID { get; set; }

        [ForeignKey("User")]
        public int User_ID { get; set; } // Foreign Key

        public string Admin_Level { get; set; }

        public string Privelages { get; set; }
        public string User_Type { get; set; }
        public DateTime? Last_Login { get; set; }
        
        public User User { get; set; }

    }
}