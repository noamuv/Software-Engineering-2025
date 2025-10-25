using System;
using System.ComponentModel.DataAnnoations;

namespace Software_Engineering_2025.Models

{
    public class User
    {
        [Key]
        public int User_ID { get; set; }

        [Required, StringLength(100)]
        public string Full_Name { get; set; }

        [Required, EmailAddress]
        public string Password { get; set; }

        public string Status { get; set; }
        public string User_Type { get; set; }
        public DateTime Created_At { get; set; } = DateTime.Now
        public bool Is_Activated { get; set; }
        public string Activation_Code { get; set; }

    }
}