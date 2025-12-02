using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Software_Engineering.Models
{
    public class User
    {
        //Primary key
        [Key]
        public int User_ID { get; set; }

        //Forgein key
        [ForeignKey("User_Type")]
        public int User_Type_ID { get; set; } // e.g., "Carer_User_ID"

        public string Title { get; set; } // Make eNum 'Mr', 'Mrs', 'Ms', 'Dr'?
        public string First_Name { get; set; }
        public string Last_Name { get; set; }
        public string Email { get; set; }
        public string Password_Hash { get; set; }
        public string Status { get; set; } //'Active', 'Inactive'
        public DateTime Created_At { get; set; } //Timestamp
        public bool Is_Activated { get; set; }
        public string Activation_Code { get; set; }

        //Navigation properties (relationships)
        public User_Type User_Type { get; set; }

        //Reverse navigation properties (if User can be a Carer)
        public Carer? Carer { get; set; }
    }
}

