using System;
using System.ComponentModel.DataAnnotations;

namespace Software_Engineering.Models
{
    public class User_Type
    {
        //Primary key
        [Key]
        public int User_Type_ID { get; set; }

        public string Type_Name { get; set; } // e.g., "Carer", "Patient
        public string Description { get; set; } // Description of the user type

        //Navigation properties (relationships)
        public ICollection<User> Users { get; set; }
    }
}


