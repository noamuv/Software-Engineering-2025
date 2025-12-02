using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Software_Engineering.Models
{
    public class Carer_Access
    {
        //Primary key
        [Key]
        public int Access_ID { get; set; }

        //Foreign keys
        [ForeignKey("Carer")]
        public int Carer_User_ID { get; set; }
        [ForeignKey("Patient")]
        public int Patient_User_ID { get; set; }

        public int Permission_Level { get; set; }
        public int Granted_By_User_ID { get; set; }
        public int Revoked_By_User_ID { get; set; } // Can be null if access is not revoked
        public DateTime Granted_At { get; set; }
        public DateTime? Revoked_At { get; set; } // Nullable to indicate access may not be revoked yet

        //Navigation properties (relationships)
        [DeleteBehavior(DeleteBehavior.Restrict)]
        public Carer Carer { get; set; }

        [DeleteBehavior(DeleteBehavior.Cascade)]
        public Patient Patient { get; set; }
    }
}
