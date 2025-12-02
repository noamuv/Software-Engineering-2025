using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Software_Engineering.Models
{
    public class Patient
    {
        //Primary and foreign key
        [Key]
        [ForeignKey("User")]
        public int User_ID { get; set; }

        //Foreign keys
        //Clinician_User_ID can be added here if needed in future
        [ForeignKey("Gender")]
        public int Gender_ID { get; set; }
        public DateTime Date_of_Birth { get; set; }

        //!!Make nullable in database
        public string Notes { get; set; }

        //Navigation properties (relationships)
        public User User { get; set; }
        public ICollection<Carer_Access> CarerAccess { get; set; }
        public Gender Gender { get; set; }
    }

}
