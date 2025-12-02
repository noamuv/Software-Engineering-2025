using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Software_Engineering.Models
{
    public class Carer
    {
        //Primary and foreign key
        [Key]
        [ForeignKey("User")]
        public int User_ID { get; set; }

        public string? Availability_Schedule { get; set; } //? indicates that the property can be null

        //Navigation properties (relationships)
        public User User { get; set; }

        //Collection (a carer can have multiple access records)
        public ICollection<Carer_Access> Carer_Accesses { get; set; }
    }
}

