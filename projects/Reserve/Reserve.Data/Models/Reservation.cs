using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using JetBrains.Annotations;
using Reserve.Data.Traits;

namespace Reserve.Data.Models
{
    [Table("reservations")]
    public class Reservation : BaseEntity, ITimestamped
    {
        public long AccountId { get; set; }
        
        public long EventId { get; set; }
        
        public long AssigneeId { get; set; }
        
        public virtual Account Account { get; set; }
        
        public virtual ReservationEvent Event { get; set; }
        
        public virtual Account Assignee { get; set; }

        public virtual ICollection<ReservedService> Services { get; set; }
        
        [Column(TypeName = "TIMESTAMP")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedAt { get; set; }
        
        [Column(TypeName = "TIMESTAMP")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime UpdatedAt { get; set; }
    }
}