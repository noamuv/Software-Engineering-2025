using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Software_Engineering.Models
{
    public class Gender
    {
        //Primary and foreign key
        [Key]
        [ForeignKey("Patient")]
        public int Gender_ID { get; set; }

        public string Name { get; set; } //UNIQUE

        //Navigation properties (relationships)
        public ICollection<Patient> Patients { get; set; }
    }
}


